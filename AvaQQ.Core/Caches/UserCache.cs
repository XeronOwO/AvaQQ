using AvaQQ.Core.Adapters;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.CacheConfiguration>;

namespace AvaQQ.Core.Caches;

internal class UserCache(
	ILogger<UserCache> logger,
	IAdapterProvider adapterProvider
	) : IUserCache
{
	private class Cache
	{
		public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;

		public bool RequiresUpdate
			=> DateTime.Now - LastUpdateTime > Config.Instance.UserUpdateInterval;

		public UserInfo? Info { get; set; } = null;
	}

	private readonly ConcurrentDictionary<ulong, Cache> _caches = [];

	private async Task<UserInfo?> GetUserInfoAsyncInternal(ulong uin)
	{
		if (adapterProvider.Adapter is not { } adapter)
		{
			return null;
		}

		try
		{
			return await adapter.GetUserInfoAsync(uin);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to get user info of user {Uin}.", uin);
			return null;
		}
	}

	public async Task<UserInfo?> GetUserInfoAsync(ulong uin, bool noCache = false)
	{
		var cache = _caches.GetOrAdd(uin, _ => new());

		if (noCache || cache.RequiresUpdate)
		{
			var info = await GetUserInfoAsyncInternal(uin);
			cache.LastUpdateTime = DateTime.Now;
			cache.Info = info;
			return info;
		}

		return cache.Info;
	}
}
