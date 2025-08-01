using AvaQQ.Entities;
using AvaQQ.Events;
using AvaQQ.SDK;
using AvaQQ.SDK.Entities;
using AvaQQ.SDK.Utils;
using Microsoft.Extensions.Logging;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Configurations.CacheConfiguration>;

#pragma warning disable IDE0079
#pragma warning disable CA1868

namespace AvaQQ.Caches;

public class UserCache : Cache<ulong, IUserInfo>, IDisposable
{
	private readonly ILogger<UserCache> _logger;

	private readonly IAdapterProvider _adapterProvider;

	private readonly IAppLifetime _lifetime;

	private readonly IDatabase _database;

	private readonly AvaQQEvents _events;

	public UserCache(ILogger<UserCache> logger, IAdapterProvider adapterProvider, IAppLifetime lifetime, IDatabase database, AvaQQEvents events)
		: base(Config.Instance.UserExpiration)
	{
		_logger = logger;
		_adapterProvider = adapterProvider;
		_lifetime = lifetime;
		_database = database;
		_events = events;

		events.OnAdaptedUserInfoFetched.Subscribe(OnAdaptedUserInfoFetched);
	}

	private readonly HashSet<ulong> _initialized = [];

	private readonly ReaderWriterLockSlim _lock = new();

	protected override void OnUpdateRequested(ulong key, IUserInfo? value)
	{
		using var lock1 = _lock.UseUpgradeableReadLock();

		if (!_initialized.Contains(key))
		{
			using var lock2 = _lock.UseWriteLock();
			if (!_initialized.Contains(key))
			{
				_initialized.Add(key);
				Task.Run(() => FetchFromDatabaseThenAdapterAsync(key, _lifetime.Token), _lifetime.Token);
			}
			return;
		}

		_events.OnAdaptedUserInfoFetched.Invoke(key, () => FetchFromAdapterAsync(key, _lifetime.Token));
	}

	private async Task FetchFromDatabaseThenAdapterAsync(ulong uin, CancellationToken token)
	{
		try
		{
			_logger.LogInformation("Fetching user info of {Uin} from database", uin);
			var newInfo = await _database.GetUserAsync(uin, token);
			if (newInfo is null)
			{
				_logger.LogInformation("User info of {Uin} not found in database", uin);
				_events.OnAdaptedUserInfoFetched.Invoke(uin, () => FetchFromAdapterAsync(uin, _lifetime.Token));
				return;
			}

			var isUpdated = false;
			AddOrUpdate(
				uin,
				_ =>
				{
					isUpdated = true;
					return newInfo;
				},
				(_, oldInfo) =>
				{
					if (newInfo.UpdateTime > oldInfo.UpdateTime)
					{
						isUpdated = true;
						return newInfo;
					}
					return oldInfo;
				});

			if (isUpdated)
			{
				_events.OnUserInfoFetched.Invoke(uin, newInfo);
			}
			_logger.LogInformation("Fetched user info of {Uin} from database {@UserInfo}", uin, newInfo);

			_events.OnAdaptedUserInfoFetched.Invoke(uin, () => FetchFromAdapterAsync(uin, _lifetime.Token));
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Operation cancelled: fetching user info of {Uin} from database", uin);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to fetch user info of {Uin} from database", uin);
			_events.OnAdaptedUserInfoFetched.Invoke(uin, () => FetchFromAdapterAsync(uin, _lifetime.Token));
		}
	}

	private async Task<AdaptedUserInfo?> FetchFromAdapterAsync(ulong uin, CancellationToken token)
	{
		if (_adapterProvider.Adapter is not { } adapter)
		{
			_logger.LogWarning("No adapter available to fetch user info of {Uin}", uin);
			return null;
		}

		try
		{
			_logger.LogInformation("Fetching user info of {Uin} from adapter", uin);
			var info = await adapter.GetUserAsync(uin, token);
			_logger.LogInformation("Fetched user info of {Uin} from adapter {UserInfo}", uin, info);
			return info;
		}
		catch (OperationCanceledException)
		{
			_logger.LogInformation("Operation cancelled: fetching user info of {Uin} from adapter", uin);
			return null;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to fetch user info of {Uin} from adapter", uin);
			return null;
		}
	}

	private void OnAdaptedUserInfoFetched(object? sender, KeyedEventBusArgs<ulong, AdaptedUserInfo?> e)
	{
		if (e.Result is not { } result)
		{
			return;
		}

		IUserInfo i = null!;
		AddOrUpdate(
			e.Key,
			_ => i = UserInfo.FromAdapted(result),
			(_, info) => i = info.Update(result));
		_events.OnUserInfoFetched.Invoke(e.Key, i);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_events.OnAdaptedUserInfoFetched.Unsubscribe(OnAdaptedUserInfoFetched);

			if (disposing)
			{
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
}
