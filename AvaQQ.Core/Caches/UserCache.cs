using AvaQQ.Core.Adapters;
using AvaQQ.Core.Databases;
using AvaQQ.Core.Events;
using AvaQQ.Core.Utils;
using Microsoft.Extensions.Logging;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.CacheConfiguration>;

namespace AvaQQ.Core.Caches;

internal class UserCache : IUserCache
{
	private readonly ILogger<UserCache> _logger;

	private readonly IAdapterProvider _adapterProvider;

	private readonly Database _database;

	private readonly EventStation _events;

	public UserCache(
		ILogger<UserCache> logger,
		IAdapterProvider adapterProvider,
		Database database,
		EventStation events
		)
	{
		_logger = logger;
		_adapterProvider = adapterProvider;
		_database = database;
		_events = events;

		_events.GetAllRecordedUsers.OnDone += OnGetAllRecordedUsers;
		_events.GetUser.OnDone += OnGetUser;
		_events.GetAllFriends.OnDone += OnGetAllFriends;
	}

	~UserCache()
	{
		_events.GetAllRecordedUsers.OnDone -= OnGetAllRecordedUsers;
		_events.GetUser.OnDone -= OnGetUser;
		_events.GetAllFriends.OnDone -= OnGetAllFriends;

		Dispose(disposing: false);
	}

	private DateTime _getAllFriendsLastUpdateTime = DateTime.MinValue;

	private bool GetAllFriendsRequiresUpdate
		=> _getAllFriendsLastUpdateTime + Config.Instance.FriendUpdateInterval < DateTime.Now;

	private class UserInfoCache
	{
		public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;

		public CachedUserInfo? Info { get; set; } = null;

		public bool RequiresUpdate
			=> LastUpdateTime + Config.Instance.UserUpdateInterval < DateTime.Now;
	}

	private int _isLocalUserInfosLoaded = 0;

	private readonly ReaderWriterLockSlim _lock = new();

	private readonly Dictionary<ulong, UserInfoCache> _caches = [];

	private void LoadLocalUserInfos()
	{
		if (Interlocked.CompareExchange(ref _isLocalUserInfosLoaded, 1, 0) != 0)
		{
			return;
		}

		_events.GetAllRecordedUsers.Enqueue(
			CommonEventId.GetAllRecordedUsers,
			() => _database.GetAllRecordedUsersAsync()
			);
	}

	private void OnGetAllRecordedUsers(object? sender, BusEventArgs<CommonEventId, RecordedUserInfo[]> e)
	{
		using var _ = _lock.UseWriteLock();
		foreach (var user in e.Result)
		{
			if (!_caches.TryGetValue(user.Uin, out var cache))
			{
				_caches.Add(user.Uin, cache = new());
			}

			cache.Info ??= new()
			{
				Uin = user.Uin,
				Nickname = user.Nickname,
				Remark = user.Remark,
			};
			cache.Info.HasLocalData = true;
		}
	}

	public CachedUserInfo[] GetUsers(Func<CachedUserInfo, bool> predicate, bool forceUpdate = false)
	{
		try
		{
			LoadLocalUserInfos();

			var adapter = _adapterProvider.EnsuredAdapter;
			if (forceUpdate || GetAllFriendsRequiresUpdate)
			{
				_events.GetAllFriends.Enqueue(
					CommonEventId.GetAllFriends,
					() => adapter.GetAllFriendsAsync()
				);
			}

			using var _ = _lock.UseReadLock();
			return [.._caches.Values
				.Where(v => v.Info != null && predicate(v.Info))
				.Select(v => v.Info!)];
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get users.");
			return [];
		}
	}

	private void OnGetAllFriends(object? sender, BusEventArgs<CommonEventId, AdaptedUserInfo[]> e)
	{
		var now = DateTime.Now;
		var users = new List<CachedUserInfo>();
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
						Nickname = info.Nickname,
						Remark = info.Remark,
					};
				}
				else
				{
					cache.Info.Nickname = info.Nickname;
					cache.Info.Remark = info.Remark;
				}
				cache.Info.IsFriend = true;
				users.Add(cache.Info);
			}

			_getAllFriendsLastUpdateTime = now;
		}

		_events.CachedGetAllFriends.DoneManually(CommonEventId.CachedGetAllFriends, [.. users]);
	}

	public CachedUserInfo? GetUser(ulong uin, bool forceUpdate = false)
	{
		try
		{
			using var _ = _lock.UseUpgradeableReadLock();
			if (!_caches.TryGetValue(uin, out var cache))
			{
				using var __ = _lock.UseWriteLock();
				_caches.Add(uin, cache = new());
			}

			if (forceUpdate || cache.RequiresUpdate)
			{
				_events.GetUser.Enqueue(
					new() { Uin = uin },
					() => _adapterProvider.EnsuredAdapter.GetUserAsync(uin)
				);
			}

			return cache.Info;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get user info with uin {Uin}.", uin);
			return null;
		}
	}

	private void OnGetUser(object? sender, BusEventArgs<UinId, AdaptedUserInfo?> e)
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
				Nickname = info.Nickname,
				Remark = info.Remark,
			};
		}
		else
		{
			cache.Info.Nickname = info.Nickname;
			cache.Info.Remark = info.Remark;
		}
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_lock.Dispose();
			}

			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion
}
