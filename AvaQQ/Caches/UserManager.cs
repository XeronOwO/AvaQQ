using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Configurations.FriendConfiguration>;

namespace AvaQQ.Caches;

internal class UserManager : IUserManager
{
	private readonly ReaderWriterLockSlim _lock = new();

	private readonly Dictionary<long, BriefFriendInfo> _briefFriendInfos = [];

	private DateTime _lastUpdateTime = DateTime.MinValue;

	private bool RequiresUpdate
		=> DateTime.Now - _lastUpdateTime > Config.Instance.FriendListUpdateInterval;

	private async Task UpdateFriendList()
	{
		if (AppBase.Current.Adapter is not { } adapter)
		{
			return;
		}

		var friendList = await adapter.GetFriendListAsync();
		_lock.EnterWriteLock();
		_briefFriendInfos.Clear();
		foreach (var friend in friendList)
		{
			_briefFriendInfos[friend.Uin] = friend;
		}
		_lock.ExitWriteLock();

		_lastUpdateTime = DateTime.Now;
	}

	public async Task<BriefFriendInfo?> GetFriendInfoAsync(long uin)
	{
		if (RequiresUpdate)
		{
			await UpdateFriendList();
		}

		_lock.EnterReadLock();
		var result = _briefFriendInfos.TryGetValue(uin, out var info) ? info : null;
		_lock.ExitReadLock();
		return result;
	}

	public async Task<BriefFriendInfo[]> GetAllFriendInfosAsync()
	{
		if (RequiresUpdate)
		{
			await UpdateFriendList();
		}

		_lock.EnterReadLock();
		var result = _briefFriendInfos.Values.ToArray();
		_lock.ExitReadLock();
		return result;
	}
}
