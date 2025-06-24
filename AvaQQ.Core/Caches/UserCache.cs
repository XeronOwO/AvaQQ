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

		_events.OnRecordedUsersLoaded.Subscribe(OnRecordedUsersLoaded);
		_events.OnUserFetched.Subscribe(OnUserFetched);
		_events.OnFriendsFetched.Subscribe(OnFriendsFetched);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_events.OnRecordedUsersLoaded.Unsubscribe(OnRecordedUsersLoaded);
			_events.OnUserFetched.Unsubscribe(OnUserFetched);
			_events.OnFriendsFetched.Unsubscribe(OnFriendsFetched);

			if (disposing)
			{
				_lock.Dispose();
			}

			disposedValue = true;
		}
	}

	~UserCache()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion

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

	private readonly Dictionary<ulong, UserInfoCache> _friendCaches = [];

	private void LoadLocalUserInfos()
	{
		if (Interlocked.CompareExchange(ref _isLocalUserInfosLoaded, 1, 0) != 0)
		{
			return;
		}

		_events.OnRecordedUsersLoaded.Invoke(() => _database.GetAllRecordedUsersAsync());
	}

	private void OnRecordedUsersLoaded(object? sender, BusEventArgs<RecordedUserInfo[]> e)
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
		}
	}

	public CachedUserInfo[] GetUsers(Func<CachedUserInfo, bool> predicate, bool forceUpdate = false)
	{
		try
		{
			LoadLocalUserInfos();

			if (forceUpdate || GetAllFriendsRequiresUpdate)
			{
				_events.OnFriendsFetched.Invoke(() => _adapterProvider.EnsuredAdapter.GetAllFriendsAsync());
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

	public CachedUserInfo[] GetFriends(bool forceUpdate = false)
	{
		try
		{
			LoadLocalUserInfos();

			if (forceUpdate || GetAllFriendsRequiresUpdate)
			{
				_events.OnFriendsFetched.Invoke(() => _adapterProvider.EnsuredAdapter.GetAllFriendsAsync());
			}

			using var _ = _lock.UseReadLock();
			return [.._friendCaches.Values
				.Where(v => v.Info != null)
				.Select(v => v.Info!)];
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get users.");
			return [];
		}
	}

	private void OnFriendsFetched(object? sender, BusEventArgs<AdaptedUserInfo[]> e)
	{
		var now = DateTime.Now;
		using var _ = _lock.UseWriteLock();

		var friendUins = new HashSet<ulong>();
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
					IsFriend = true,
				};
				_events.OnNewUserCached.Invoke(cache.Info);
			}
			else
			{
				if (info.Nickname != cache.Info.Nickname)
				{
					var oldNickname = cache.Info.Nickname;
					var newNickname = cache.Info.Nickname = info.Nickname;
					_events.OnUserNicknameChanged.Invoke(new UserNicknameChangedInfo(
						oldNickname,
						newNickname,
						cache.Info
					));
				}

				if (info.Remark != cache.Info.Remark)
				{
					var oldRemark = cache.Info.Remark;
					var newRemark = cache.Info.Remark = info.Remark;
					_events.OnUserRemarkChanged.Invoke(new UserRemarkChangedInfo(
						oldRemark,
						newRemark,
						cache.Info
					));
				}

				if (!cache.Info.IsFriend)
				{
					cache.Info.IsFriend = true;
				}
			}

			if (_friendCaches.TryAdd(info.Uin, cache))
			{
				_events.OnFriendCacheAdded.Invoke(cache.Info);
			}
			friendUins.Add(info.Uin);
		}

		foreach (var (uin, _) in _friendCaches)
		{
			if (friendUins.Contains(uin))
			{
				continue;
			}

			if (_caches.TryGetValue(uin, out var cache) && cache.Info != null)
			{
				cache.Info.IsFriend = false;
				_events.OnFriendCacheRemoved.Invoke(cache.Info);
			}
			_friendCaches.Remove(uin);
		}

		_getAllFriendsLastUpdateTime = now;
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
				_events.OnUserFetched.Invoke(
					new(uin),
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

	private void OnUserFetched(object? sender, BusEventArgs<UinId, AdaptedUserInfo?> e)
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
			_events.OnNewUserCached.Invoke(cache.Info);
		}
		else
		{
			if (info.Nickname != cache.Info.Nickname)
			{
				var oldNickname = cache.Info.Nickname;
				var newNickname = cache.Info.Nickname = info.Nickname;
				_events.OnUserNicknameChanged.Invoke(new UserNicknameChangedInfo(
					oldNickname,
					newNickname,
					cache.Info
				));
			}

			if (info.Remark != cache.Info.Remark)
			{
				var oldRemark = cache.Info.Remark;
				var newRemark = cache.Info.Remark = info.Remark;
				_events.OnUserRemarkChanged.Invoke(new UserRemarkChangedInfo(
					oldRemark,
					newRemark,
					cache.Info
				));
			}
		}
	}
}
