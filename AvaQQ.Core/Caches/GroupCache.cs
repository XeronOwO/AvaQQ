using AvaQQ.Core.Adapters;
using AvaQQ.Core.Databases;
using AvaQQ.Core.Events;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.CacheConfiguration>;

namespace AvaQQ.Core.Caches;

internal class GroupCache : IGroupCache
{
	private readonly ILogger<GroupCache> _logger;

	private readonly IAdapterProvider _adapterProvider;

	private readonly IUserCache _userCache;

	private readonly Database _database;

	private readonly EventStation _events;

	public GroupCache(
		ILogger<GroupCache> logger,
		IAdapterProvider adapterProvider,
		IUserCache userCache,
		Database database,
		EventStation events
		)
	{
		_logger = logger;
		_adapterProvider = adapterProvider;
		_userCache = userCache;
		_database = database;
		_events = events;

		_events.GetAllJoinedGroups.OnDone += OnGetAllJoinedGroups;
		_events.GetJoinedGroup.OnDone += OnGetJoinedGroup;
	}

	~GroupCache()
	{
		_events.GetAllJoinedGroups.OnDone -= OnGetAllJoinedGroups;
		_events.GetJoinedGroup.OnDone -= OnGetJoinedGroup;
	}

	private DateTime _getAllJoinedGroupsLastUpdateTime = DateTime.MinValue;

	private bool GetAllJoinedGroupsRequiresUpdate
		=> _getAllJoinedGroupsLastUpdateTime + Config.Instance.GroupUpdateInterval < DateTime.Now;

	private struct GroupInfoCache
	{
		public required DateTime LastUpdateTime { get; set; }

		public required CachedGroupInfo? Info { get; set; }

		public readonly bool RequiresUpdate
			=> LastUpdateTime + Config.Instance.GroupUpdateInterval < DateTime.Now;
	}

	private readonly ConcurrentDictionary<ulong, GroupInfoCache> _groupInfoCaches = [];

	private int _isLocalGroupInfosLoaded = 0;

	private void LoadLocalGroupInfos()
	{
		if (Interlocked.CompareExchange(ref _isLocalGroupInfosLoaded, 1, 0) != 0)
		{
			return;
		}

		foreach (var info in _database.GetAllRecordedGroups())
		{
			var cache = new GroupInfoCache
			{
				LastUpdateTime = DateTime.Now,
				Info = info,
			};
			cache.Info.HasLocalData = true;
			_groupInfoCaches[info.Uin] = cache;
		}
	}

	public CachedGroupInfo[] GetGroups(Func<CachedGroupInfo, bool> predicate, bool forceUpdate = false)
	{
		try
		{
			LoadLocalGroupInfos();

			if (forceUpdate || GetAllJoinedGroupsRequiresUpdate)
			{
				_events.GetAllJoinedGroups.Enqueue(
					CommonEventId.GetAllJoinedGroups,
					_adapterProvider.EnsuredAdapter.GetAllJoinedGroupsAsync()
				);
			}

			return [.. _groupInfoCaches.Values
				.Where(v => v.Info != null && predicate(v.Info))
				.Select(v => v.Info!)];
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get groups.");
			return [];
		}
	}

	public CachedGroupInfo? GetJoinedGroup(ulong uin, bool forceUpdate = false)
	{
		try
		{
			LoadLocalGroupInfos();

			var cache = _groupInfoCaches.GetOrAdd(uin, (_) => new GroupInfoCache
			{
				LastUpdateTime = DateTime.MinValue,
				Info = null,
			});

			if (forceUpdate || cache.RequiresUpdate)
			{
				_events.GetJoinedGroup.Enqueue(
					new() { Uin = uin },
					_adapterProvider.EnsuredAdapter.GetJoinedGroupAsync(uin)
				);
			}

			return cache.Info;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get all joined groups.");
			return null;
		}
	}

	private void OnGetAllJoinedGroups(object? sender, BusEventArgs<CommonEventId, AdaptedGroupInfo[]> e)
	{
		var now = DateTime.Now;
		var groups = new List<CachedGroupInfo>();
		foreach (var info in e.Result)
		{
			var cache = _groupInfoCaches.AddOrUpdate(info.Uin, (_) =>
			{
				var cache = new GroupInfoCache
				{
					LastUpdateTime = now,
					Info = info,
				};
				cache.Info.IsStillIn = true;
				return cache;
			}, (_, cache) =>
			{
				cache.LastUpdateTime = now;
				if (cache.Info == null)
				{
					cache.Info = info;
				}
				else
				{
					cache.Info.Update(info);
				}
				cache.Info.IsStillIn = true;
				return cache;
			});
			groups.Add(cache.Info!);
		}
		_getAllJoinedGroupsLastUpdateTime = now;

		_events.CachedGetAllJoinedGroups.DoneManually(CommonEventId.CachedGetAllJoinedGroups, [.. groups]);
	}

	private void OnGetJoinedGroup(object? sender, BusEventArgs<UinId, AdaptedGroupInfo?> e)
	{
		var now = DateTime.Now;
		var info = e.Result;
		var cache = _groupInfoCaches.AddOrUpdate(e.Id.Uin, (_) =>
		{
			var cache = new GroupInfoCache
			{
				LastUpdateTime = now,
				Info = info,
			};
			if (cache.Info != null)
			{
				cache.Info.IsStillIn = true;
			}
			return cache;
		}, (_, cache) =>
		{
			cache.LastUpdateTime = now;
			if (cache.Info == null)
			{
				cache.Info = info;
			}
			else
			{
				cache.Info.Update(info);
			}
			if (cache.Info != null)
			{
				cache.Info.IsStillIn = true;
			}
			return cache;
		});

		_events.CachedGetJoinedGroup.DoneManually(new() { Uin = e.Id.Uin }, cache.Info);
	}
}
