using AvaQQ.Core.Adapters;
using AvaQQ.Core.Databases;
using AvaQQ.Core.Messages;
using AvaQQ.Core.Resources;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.CacheConfiguration>;

namespace AvaQQ.Core.Caches;

internal class GroupCache(
	ILogger<GroupCache> logger,
	IAdapterProvider adapterProvider,
	IFriendCache friendCache,
	IUserCache userCache,
	GroupMessageDatabase groupMessageDatabase
	) : IGroupCache
{
	#region 群

	#region 群信息

	private readonly ReaderWriterLockSlim _groupsLock = new();

	private readonly Dictionary<ulong, GroupInfo> _groups = [];

	private DateTime _groupsLastUpdateTime = DateTime.MinValue;

	private bool GroupsRequiresUpdate
		=> DateTime.Now - _groupsLastUpdateTime > Config.Instance.GroupUpdateInterval;

	public async Task UpdateGroupListAsync()
	{
		try
		{
			if (adapterProvider.Adapter is not { } adapter)
			{
				throw new InvalidOperationException("Adapter is not available.");
			}

			var groupList = await adapter.GetGroupListAsync();
			_groupsLock.EnterWriteLock();
			_groups.Clear();
			foreach (var group in groupList)
			{
				_groups[group.Uin] = group;
			}
			_groupsLock.ExitWriteLock();

			_groupNames.Clear();

			_groupsLastUpdateTime = DateTime.Now;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to update group list.");
		}
	}

	private GroupInfo? GetGroupInfoInternal(ulong uin)
	{
		_groupsLock.EnterReadLock();
		var result = _groups.TryGetValue(uin, out var info) ? info : null;
		_groupsLock.ExitReadLock();
		return result;
	}

	private async Task<GroupInfo?> GetGroupInfoInternalAsync(ulong uin)
	{
		await UpdateGroupListAsync();

		return GetGroupInfoInternal(uin);
	}

	public Task<GroupInfo?> GetGroupInfoAsync(ulong uin, bool noCache = false)
	{
		if (!GroupsRequiresUpdate
			&& !noCache)
		{
			return Task.FromResult(GetGroupInfoInternal(uin));
		}

		return GetGroupInfoInternalAsync(uin);
	}

	private IEnumerable<GroupInfo> GetAllGroupInfosInternal()
	{
		_groupsLock.EnterReadLock();
		var result = _groups.Values;
		_groupsLock.ExitReadLock();
		return result;
	}

	private async Task<IEnumerable<GroupInfo>> GetAllGroupInfosInternalAsync()
	{
		await UpdateGroupListAsync();

		return GetAllGroupInfosInternal();
	}

	public Task<IEnumerable<GroupInfo>> GetAllGroupInfosAsync(bool noCache = false)
	{
		if (!GroupsRequiresUpdate
			&& !noCache)
		{
			return Task.FromResult(GetAllGroupInfosInternal());
		}

		return GetAllGroupInfosInternalAsync();
	}

	#endregion

	#region 群名

	private readonly ConcurrentDictionary<ulong, Task<string>> _groupNames = [];

	private async Task<string> GetGroupNameInternalAsync(ulong uin)
	{
		if (await GetGroupInfoAsync(uin) is { } group)
		{
			return group.Name;
		}

		return string.Empty;
	}

	public Task<string> GetGroupNameAsync(ulong uin)
		=> _groupNames.GetOrAdd(uin, GetGroupNameInternalAsync);

	#endregion

	#endregion

	#region 群成员信息

	private async Task<Dictionary<ulong, GroupMemberInfo>> GetAllGroupMemberInfosInternalAsync(ulong groupUin)
	{
		try
		{
			if (adapterProvider.Adapter is not { } adapter)
			{
				throw new InvalidOperationException("Adapter is not available.");
			}

			var memberList = await adapter.GetGroupMemberListAsync(groupUin);
			var result = new Dictionary<ulong, GroupMemberInfo>();
			foreach (var member in memberList)
			{
				result[member.Uin] = member;
			}
			return result;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to update group member list of group {GroupUin}.", groupUin);
			return [];
		}
	}

	private class MemberCache(Task<Dictionary<ulong, GroupMemberInfo>> task)
	{
		public DateTime LastUpdateTime { get; set; } = DateTime.Now;

		public Task<Dictionary<ulong, GroupMemberInfo>> Task { get; set; } = task;

		public bool RequiresUpdate
			=> DateTime.Now - LastUpdateTime > Config.Instance.GroupMemberUpdateInterval;
	}

	private readonly ReaderWriterLockSlim _memberCacheLock = new();

	private readonly Dictionary<ulong, MemberCache> _memberCaches = [];

	public Task<Dictionary<ulong, GroupMemberInfo>> GetAllGroupMemberInfosAsync(ulong groupUin, bool noCache = false)
	{
		_memberCacheLock.EnterReadLock();

		if (!_memberCaches.TryGetValue(groupUin, out var cache)
			|| cache.RequiresUpdate)
		{
			_memberCacheLock.ExitReadLock();

			_memberCacheLock.EnterWriteLock();
			var task = GetAllGroupMemberInfosInternalAsync(groupUin);
			_memberCaches[groupUin] = new MemberCache(task);
			_memberCacheLock.ExitWriteLock();

			return task;
		}

		_memberCacheLock.ExitReadLock();

		return cache.Task;
	}

	public async Task<string> ResolveGroupMemberDisplayNameAsync(ulong groupUin, ulong memberUin, bool noCache = false)
	{
		var friend = await friendCache.GetFriendInfoAsync(memberUin, noCache);
		if (friend is not null
			&& !string.IsNullOrEmpty(friend.Remark))
		{
			return friend.Remark; // 优先备注名
		}

		if ((await GetAllGroupMemberInfosAsync(groupUin, noCache)).TryGetValue(memberUin, out var member)
			&& !string.IsNullOrEmpty(member.Card))
		{
			return member.Card; // 其次群名片
		}

		if (friend is not null)
		{
			return friend.Nickname; // 最后昵称
		}

		if (await userCache.GetUserInfoAsync(memberUin, noCache) is { } user)
		{
			return user.Nickname;
		}

		logger.LogWarning("Failed to resolve display name of group member {MemberUin} in group {GroupUin}.", memberUin, groupUin);

		return string.Empty;
	}

	#endregion

	#region 消息

	#region 消息

	private readonly ConcurrentDictionary<ulong, GroupMessageEntry?> _latestMessages = [];

	private GroupMessageEntry? GetLatestMessageEntryInternal(ulong group)
		=> groupMessageDatabase.Latest(group);

	public GroupMessageEntry? GetLatestMessageEntry(ulong group)
		=> _latestMessages.GetOrAdd(group, GetLatestMessageEntryInternal);

	private bool TryUpdateLatestMessage(GroupMessageEventArgs e)
	{
		if (_latestMessages.TryGetValue(e.GroupUin, out var entry)
			&& entry is not null
			&& entry.Time > e.Time)
		{
			return false;
		}

		_latestMessages[e.GroupUin] = e;
		_latestMessageTimes.TryRemove(e.GroupUin, out _);
		_latestMessagePreviews.TryRemove(e.GroupUin, out _);

		OnUpdated?.Invoke(this, e);

		return true;
	}

	#endregion

	#region 时间

	private readonly ConcurrentDictionary<ulong, string> _latestMessageTimes = [];

	private string GetLatestMessageTimeInternal(ulong group)
	{
		if (GetLatestMessageEntry(group) is not { } entry)
		{
			return string.Empty;
		}

		return entry.Time.ToLocalTime().ToString("HH:mm");
	}

	public string GetLatestMessageTime(ulong group)
		=> _latestMessageTimes.GetOrAdd(group, GetLatestMessageTimeInternal);

	#endregion

	#region 预览

	private async Task<string> GenerateMessagePreviewAsync(ulong group, ulong member, Message message)
	{
		var sb = new StringBuilder();

		if (adapterProvider.Adapter is { } adapter
			&& adapter.Uin != member)
		{
			sb.Append(await ResolveGroupMemberDisplayNameAsync(group, member));
			sb.Append(": ");
		}

		foreach (var segment in message)
		{
			switch (segment)
			{
				case AtSegment at:
					sb.Append('@');
					sb.Append(await ResolveGroupMemberDisplayNameAsync(group, at.Uin));
					break;
				case FaceSegment:
					sb.Append(SR.FaceSegmentPreview);
					break;
				case ForwardSegment:
					sb.Append(SR.ForwardSegmentPreview);
					break;
				case ImageSegment image:
					sb.Append(image.Summary);
					break;
				case MarketFaceSegment marketFace:
					sb.Append(marketFace.Summary);
					break;
				case ReplySegment:
					break;
				case TextSegment { Text: { } } text:
					sb.Append(
						text.Text
						.Replace("\r\n", " ")
						.Replace("\n", " ")
						);
					break;
				default:
					logger.LogWarning("Unsupported message preview segment type {Type}.", segment.GetType());
					break;
			}
		}

		return sb.ToString();
	}

	private Task<string> GenerateMessagePreviewAsync(ulong groupUin, GroupMessageEntry entry)
		=> GenerateMessagePreviewAsync(groupUin, entry.SenderUin, entry.Message);

	private readonly ConcurrentDictionary<ulong, Task<string>> _latestMessagePreviews = [];

	private async Task<string> GetLatestMessagePreviewInternalAsync(ulong group)
	{
		if (GetLatestMessageEntry(group) is not { } entry)
		{
			return string.Empty;
		}

		return await GenerateMessagePreviewAsync(group, entry);
	}

	public Task<string> GetLatestMessagePreviewAsync(ulong group)
		=> _latestMessagePreviews.GetOrAdd(group, GetLatestMessagePreviewInternalAsync);

	#endregion

	#region 同步

	public void StartMessageSyncTask(CancellationToken token)
		=> Task.Run(() => SyncMessageAsync(token), token);

	private async Task SyncMessageAsync(CancellationToken token)
	{
		try
		{
			await PreSyncMessageAsync(token);
			await PostSyncMessageAsync(token);

			logger.LogInformation("Message sync task finished successfully.");
		}
		catch (OperationCanceledException)
		{
			logger.LogInformation("Message sync task has been canceled.");
		}
		catch (Exception e)
		{
			logger.LogError(e, "Message sync task exited unexpectedly.");
		}
	}

	public event EventHandler? OnUpdated;

	/// <summary>
	/// 预同步消息<br/>
	/// 只会同步部分最新消息，不会同步全部历史消息
	/// </summary>
	private async Task PreSyncMessageAsync(CancellationToken token)
	{
		if (adapterProvider.Adapter is not { } adapter)
		{
			throw new InvalidOperationException("Adapter is not available.");
		}

		foreach (var group in await GetAllGroupInfosAsync())
		{
			var eventArgs = await adapter.GetGroupMessageHistoryAsync(group.Uin, 0, Config.Instance.PreSyncMessageCount);

			groupMessageDatabase.Sync(group.Uin, eventArgs.Select(e => (GroupMessageEntry)e));

			foreach (var eventArg in eventArgs)
			{
				TryUpdateLatestMessage(eventArg);
			}
		}
	}

	/// <summary>
	/// 后同步消息<br/>
	/// 同步全部历史消息
	/// </summary>
	private Task PostSyncMessageAsync(CancellationToken token)
	{
		// TODO: 同步全部历史消息

		return Task.CompletedTask;
	}

	#endregion

	public void Adapter_OnGroupMessage(object? sender, GroupMessageEventArgs e)
	{
		TryUpdateLatestMessage(e);

		groupMessageDatabase.Insert(e.GroupUin, e);
	}

	#endregion
}
