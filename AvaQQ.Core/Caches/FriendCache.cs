using AvaQQ.Core.Adapters;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.CacheConfiguration>;

namespace AvaQQ.Core.Caches;

internal class FriendCache(
	ILogger<FriendCache> logger,
	IAdapterProvider adapterProvider
	) : IFriendCache
{
	private readonly ConcurrentDictionary<ulong, FriendInfo> _infos = [];

	private DateTime _lastUpdateTime = DateTime.MinValue;

	private bool RequiresUpdate
		=> DateTime.Now - _lastUpdateTime > Config.Instance.FriendUpdateInterval;

	private async Task UpdateFriendListAsync()
	{
		if (adapterProvider.Adapter is not { } adapter)
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
