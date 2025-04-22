using AvaQQ.Core.Adapters;
using AvaQQ.Core.Databases;
using AvaQQ.Core.Events;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
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

		_events.GetUser.OnDone += OnGetUser;
		_events.GetAllFriends.OnDone += OnGetAllFriends;
	}

	~UserCache()
	{
		_events.GetUser.OnDone -= OnGetUser;
		_events.GetAllFriends.OnDone -= OnGetAllFriends;
	}

	private DateTime _getAllFriendsLastUpdateTime = DateTime.MinValue;

	private bool GetAllFriendsRequiresUpdate
		=> _getAllFriendsLastUpdateTime + Config.Instance.FriendUpdateInterval < DateTime.Now;

	private struct UserInfoCache
	{
		public required DateTime LastUpdateTime { get; set; }

		public required CachedUserInfo? Info { get; set; }

		public readonly bool RequiresUpdate
			=> LastUpdateTime + Config.Instance.UserUpdateInterval < DateTime.Now;
	}

	private readonly ConcurrentDictionary<ulong, UserInfoCache> _userInfoCaches = [];

	private int _isLocalUserInfosLoaded = 0;

	private void LoadLocalUserInfos()
	{
		if (Interlocked.CompareExchange(ref _isLocalUserInfosLoaded, 1, 0) != 0)
		{
			return;
		}

		foreach (var info in _database.GetAllRecordedUsers())
		{
			var cache = new UserInfoCache
			{
				LastUpdateTime = DateTime.Now,
				Info = info,
			};
			cache.Info.HasLocalData = true;
			_userInfoCaches[info.Uin] = cache;
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
					adapter.GetAllFriendsAsync()
				);
			}

			return [.. _userInfoCaches.Values
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
		foreach (var info in e.Result)
		{
			var cache = _userInfoCaches.AddOrUpdate(info.Uin, (_) =>
			{
				var cache = new UserInfoCache
				{
					LastUpdateTime = now,
					Info = info,
				};
				cache.Info.IsFriend = true;
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
				cache.Info.IsFriend = true;
				return cache;
			});
			users.Add(cache.Info!);
		}
		_getAllFriendsLastUpdateTime = now;

		_events.CachedGetAllFriends.DoneManually(CommonEventId.CachedGetAllFriends, [.. users]);
	}

	public CachedUserInfo? GetUser(ulong uin, bool forceUpdate = false)
	{
		try
		{
			var cache = _userInfoCaches.GetOrAdd(uin, (_) => new UserInfoCache
			{
				LastUpdateTime = DateTime.MinValue,
				Info = null,
			});

			if (forceUpdate || cache.RequiresUpdate)
			{
				_events.GetUser.Enqueue(
					new() { Uin = uin },
					_adapterProvider.EnsuredAdapter.GetUserAsync(uin)
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
		_userInfoCaches.AddOrUpdate(e.Id.Uin, (_) =>
		{
			return new()
			{
				LastUpdateTime = DateTime.Now,
				Info = e.Result,
			};
		}, (u, cache) =>
		{
			cache.LastUpdateTime = DateTime.Now;
			if (cache.Info == null)
			{
				cache.Info = e.Result;
			}
			else
			{
				cache.Info.Update(e.Result);
			}
			return cache;
		});
	}
}
