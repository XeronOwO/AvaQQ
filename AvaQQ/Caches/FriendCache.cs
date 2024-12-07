using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Configurations.CacheConfiguration>;

namespace AvaQQ.Caches;

internal class FriendCache(
	ILogger<FriendCache> logger
	) : IFriendCache
{
	private readonly ConcurrentDictionary<ulong, FriendInfo> _infos = [];

	private DateTime _lastUpdateTime = DateTime.MinValue;

	private bool RequiresUpdate
		=> DateTime.Now - _lastUpdateTime > Config.Instance.FriendUpdateInterval;

	private async Task UpdateFriendListAsync()
	{
		if (AppBase.Current.Adapter is not { } adapter)
		{
			return;
		}

		try
		{
			var friendList = await adapter.GetFriendListAsync();
			_infos.Clear();
			foreach (var friend in friendList)
			{
				_infos[friend.Uin] = friend;
			}

			_lastUpdateTime = DateTime.Now;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to update friend list.");
		}
	}

	public async Task<FriendInfo?> GetFriendInfoAsync(ulong uin, bool noCache = false)
	{
		if (noCache || RequiresUpdate)
		{
			await UpdateFriendListAsync();
		}

		return _infos.TryGetValue(uin, out var info) ? info : null;
	}

	public async Task<FriendInfo[]> GetAllFriendInfosAsync(bool noCache = false)
	{
		if (noCache || RequiresUpdate)
		{
			await UpdateFriendListAsync();
		}

		return [.. _infos.Values];
	}
}
