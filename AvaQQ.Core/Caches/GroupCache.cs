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

		_events.OnRecordedGroupsLoaded.Subscribe(OnRecordedGroupsLoaded);
		_events.OnJoinedGroupsFetched.Subscribe(OnJoinedGroupsFetched);
		_events.OnJoinedGroupFetched.Subscribe(OnJoinedGroupFetched);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_events.OnRecordedGroupsLoaded.Unsubscribe(OnRecordedGroupsLoaded);
			_events.OnJoinedGroupsFetched.Unsubscribe(OnJoinedGroupsFetched);
			_events.OnJoinedGroupFetched.Unsubscribe(OnJoinedGroupFetched);

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

	private readonly Dictionary<ulong, GroupInfoCache> _joinedGroupCaches = [];

	private void LoadLocalGroupInfos()
	{
		if (Interlocked.CompareExchange(ref _isLocalGroupInfosLoaded, 1, 0) != 0)
		{
			return;
		}

		_events.OnRecordedGroupsLoaded.Invoke(() => _database.GetAllRecordedGroupsAsync());
	}

	private void OnRecordedGroupsLoaded(object? sender, BusEventArgs<RecordedGroupInfo[]> e)
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
		}
	}

	public CachedGroupInfo[] GetGroups(Func<CachedGroupInfo, bool> predicate, bool forceUpdate = false)
	{
		try
		{
			LoadLocalGroupInfos();

			if (forceUpdate || GetAllJoinedGroupsRequiresUpdate)
			{
				_events.OnJoinedGroupsFetched.Invoke(() => _adapterProvider.EnsuredAdapter.GetAllJoinedGroupsAsync());
			}

			using var _ = _lock.UseReadLock();
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

	public CachedGroupInfo[] GetJoinedGroups(bool forceUpdate = false)
	{
		try
		{
			LoadLocalGroupInfos();

			if (forceUpdate || GetAllJoinedGroupsRequiresUpdate)
			{
				_events.OnJoinedGroupsFetched.Invoke(() => _adapterProvider.EnsuredAdapter.GetAllJoinedGroupsAsync());
			}

			using var _ = _lock.UseReadLock();
			return [.. _joinedGroupCaches.Values
					.Where(v => v.Info != null)
					.Select(v => v.Info!)];
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get groups.");
			return [];
		}
	}

	private void OnJoinedGroupsFetched(object? sender, BusEventArgs<AdaptedGroupInfo[]> e)
	{
		var now = DateTime.Now;
		using var _ = _lock.UseWriteLock();

		var joinedGroupUins = new HashSet<ulong>();
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
					IsStillIn = true,
				};
				_events.OnNewGroupCached.Invoke(cache.Info);
			}
			else
			{
				if (info.Name != cache.Info.Name)
				{
					var oldName = cache.Info.Name;
					var newName = cache.Info.Name = info.Name;
					_events.OnGroupNameChanged.Invoke(new GroupNameChangedInfo(
						oldName,
						newName,
						cache.Info
					));
				}

				if (info.Remark != cache.Info.Remark)
				{
					var oldRemark = cache.Info.Remark;
					var newRemark = cache.Info.Remark = info.Remark;
					_events.OnGroupRemarkChanged.Invoke(new GroupRemarkChangedInfo(
						oldRemark,
						newRemark,
						cache.Info
					));
				}

				if (!cache.Info.IsStillIn)
				{
					cache.Info.IsStillIn = true;
				}
			}

			if (_joinedGroupCaches.TryAdd(info.Uin, cache))
			{
				_events.OnGroupCacheAdded.Invoke(cache.Info);
			}
			joinedGroupUins.Add(info.Uin);
		}

		foreach (var (uin, _) in _caches)
		{
			if (joinedGroupUins.Contains(uin))
			{
				continue;
			}

			if (_caches.TryGetValue(uin, out var cache) && cache.Info != null)
			{
				cache.Info.IsStillIn = false;
				_events.OnGroupCacheRemoved.Invoke(cache.Info);
			}
			_joinedGroupCaches.Remove(uin);
		}

		_getAllJoinedGroupsLastUpdateTime = now;
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
				_events.OnJoinedGroupFetched.Invoke(
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

	private void OnJoinedGroupFetched(object? sender, BusEventArgs<UinId, AdaptedGroupInfo?> e)
	{
		var uin = e.Id.Uin;

		var now = DateTime.Now;
		using var _ = _lock.UseWriteLock();
		if (!_caches.TryGetValue(uin, out var cache))
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
			_events.OnNewGroupCached.Invoke(cache.Info);
		}
		else
		{
			if (info.Name != cache.Info.Name)
			{
				var oldName = cache.Info.Name;
				var newName = cache.Info.Name = info.Name;
				_events.OnGroupNameChanged.Invoke(new GroupNameChangedInfo(
					oldName,
					newName,
					cache.Info
				));
			}

			if (info.Remark != cache.Info.Remark)
			{
				var oldRemark = cache.Info.Remark;
				var newRemark = cache.Info.Remark = info.Remark;
				_events.OnGroupRemarkChanged.Invoke(new GroupRemarkChangedInfo(
					oldRemark,
					newRemark,
					cache.Info
				));
			}
		}
	}
}
