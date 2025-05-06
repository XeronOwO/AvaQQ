using AvaQQ.Core.Adapters;
using AvaQQ.Core.Databases;
using AvaQQ.Core.Events;
using AvaQQ.Core.Utils;
using Microsoft.Extensions.Logging;
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

		_events.GetAllRecordedGroups.Subscribe(OnGetAllRecordedGroups);
		_events.GetAllJoinedGroups.Subscribe(OnGetAllJoinedGroups);
		_events.GetJoinedGroup.Subscribe(OnGetJoinedGroup);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_events.GetAllRecordedGroups.Subscribe(OnGetAllRecordedGroups);
			_events.GetAllJoinedGroups.Subscribe(OnGetAllJoinedGroups);
			_events.GetJoinedGroup.Subscribe(OnGetJoinedGroup);

			if (disposing)
			{
				_lock.Dispose();
			}

			disposedValue = true;
		}
	}

	~GroupCache()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion

	private DateTime _getAllJoinedGroupsLastUpdateTime = DateTime.MinValue;

	private bool GetAllJoinedGroupsRequiresUpdate
		=> _getAllJoinedGroupsLastUpdateTime + Config.Instance.GroupUpdateInterval < DateTime.Now;

	private class GroupInfoCache
	{
		public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;

		public CachedGroupInfo? Info { get; set; } = null;

		public bool RequiresUpdate
			=> LastUpdateTime + Config.Instance.GroupUpdateInterval < DateTime.Now;
	}

	private int _isLocalGroupInfosLoaded = 0;

	private readonly ReaderWriterLockSlim _lock = new();

	private readonly Dictionary<ulong, GroupInfoCache> _caches = [];

	private void LoadLocalGroupInfos()
	{
		if (Interlocked.CompareExchange(ref _isLocalGroupInfosLoaded, 1, 0) != 0)
		{
			return;
		}

		_events.GetAllRecordedGroups.Invoke(
			CommonEventId.GetAllRecordedGroups,
			() => _database.GetAllRecordedGroupsAsync()
			);
	}

	private void OnGetAllRecordedGroups(object? sender, BusEventArgs<CommonEventId, RecordedGroupInfo[]> e)
	{
		using var _ = _lock.UseWriteLock();
		foreach (var group in e.Result)
		{
			if (!_caches.TryGetValue(group.Uin, out var cache))
			{
				_caches.Add(group.Uin, cache = new());
			}

			cache.Info ??= new()
			{
				Uin = group.Uin,
				Name = group.Name,
				Remark = group.Remark,
			};
			cache.Info.HasLocalData = true;
		}
	}

	public CachedGroupInfo[] GetGroups(Func<CachedGroupInfo, bool> predicate, bool forceUpdate = false)
	{
		try
		{
			LoadLocalGroupInfos();

			using var _ = _lock.UseReadLock();
			if (forceUpdate || GetAllJoinedGroupsRequiresUpdate)
			{
				_events.GetAllJoinedGroups.Invoke(
					CommonEventId.GetAllJoinedGroups,
					() => _adapterProvider.EnsuredAdapter.GetAllJoinedGroupsAsync()
				);
			}

			return [.. _caches.Values
					.Where(v => v.Info != null && predicate(v.Info))
					.Select(v => v.Info!)];
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get groups.");
			return [];
		}
	}

	private void OnGetAllJoinedGroups(object? sender, BusEventArgs<CommonEventId, AdaptedGroupInfo[]> e)
	{
		var now = DateTime.Now;
		var groups = new List<CachedGroupInfo>();
		using (var _ = _lock.UseWriteLock())
		{
			foreach (var info in e.Result)
			{
				if (!_caches.TryGetValue(info.Uin, out var cache))
				{
					_caches.Add(info.Uin, cache = new());
				}

				cache.LastUpdateTime = now;
				if (cache.Info == null)
				{
					cache.Info = new()
					{
						Uin = info.Uin,
						Name = info.Name,
						Remark = info.Remark,
					};
				}
				else
				{
					cache.Info.Name = info.Name;
					cache.Info.Remark = info.Remark;
				}
				cache.Info.IsStillIn = true;
				groups.Add(cache.Info);
			}

			_getAllJoinedGroupsLastUpdateTime = now;
		}

		_events.CachedGetAllJoinedGroups.Invoke(CommonEventId.CachedGetAllJoinedGroups, [.. groups]);
	}

	public CachedGroupInfo? GetJoinedGroup(ulong uin, bool forceUpdate = false)
	{
		try
		{
			LoadLocalGroupInfos();

			using var _ = _lock.UseUpgradeableReadLock();
			if (!_caches.TryGetValue(uin, out var cache))
			{
				using var __ = _lock.UseWriteLock();
				_caches.Add(uin, cache = new());
			}

			if (forceUpdate || cache.RequiresUpdate)
			{
				_events.GetJoinedGroup.Invoke(
					new() { Uin = uin },
					() => _adapterProvider.EnsuredAdapter.GetJoinedGroupAsync(uin)
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

	private void OnGetJoinedGroup(object? sender, BusEventArgs<UinId, AdaptedGroupInfo?> e)
	{
		var uin = e.Id.Uin;

		var now = DateTime.Now;
		GroupInfoCache? cache;
		using (var _ = _lock.UseWriteLock())
		{
			if (!_caches.TryGetValue(uin, out cache))
			{
				_caches.Add(uin, cache = new());
			}
			cache.LastUpdateTime = now;

			if (e.Result is not { } info)
			{
				return;
			}

			if (cache.Info == null)
			{
				cache.Info = new()
				{
					Uin = info.Uin,
					Name = info.Name,
					Remark = info.Remark,
				};
			}
			else
			{
				cache.Info.Name = info.Name;
				cache.Info.Remark = info.Remark;
			}
			cache.Info.IsStillIn = true;
		}

		_events.CachedGetJoinedGroup.Invoke(new() { Uin = e.Id.Uin }, cache.Info);
	}
}
