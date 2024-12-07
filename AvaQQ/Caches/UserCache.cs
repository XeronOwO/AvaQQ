using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Configurations.CacheConfiguration>;

namespace AvaQQ.Caches;

internal class UserCache(
	ILogger<UserCache> logger
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
		if (AppBase.Current.Adapter is not { } adapter)
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
