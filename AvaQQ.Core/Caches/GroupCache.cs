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

	private class MemberCache
	{
		public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;

		public ConcurrentDictionary<ulong, GroupMemberInfo> Members { get; } = [];

		public bool RequiresUpdate
			=> DateTime.Now - LastUpdateTime > Config.Instance.GroupMemberUpdateInterval;
	}

	private readonly ConcurrentDictionary<ulong, MemberCache> _memberCaches = [];

	private async Task UpdateMemberListAsync(ulong groupUin)
	{
		if (adapterProvider.Adapter is not { } adapter)
		{
			return;
		}

		try
		{
			var memberList = await adapter.GetGroupMemberListAsync(groupUin);
			var cache = _memberCaches.GetOrAdd(groupUin, _ => new());
			cache.Members.Clear();
			foreach (var member in memberList)
			{
				cache.Members[member.Uin] = member;
			}
			cache.LastUpdateTime = DateTime.Now;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to update group member list of group {GroupUin}.", groupUin);
		}
	}

	public async Task<GroupMemberInfo[]> GetAllGroupMemberInfosAsync(ulong groupUin, bool noCache = false)
	{
		if (_memberCaches.TryGetValue(groupUin, out var cache)
			&& !cache.RequiresUpdate
			&& !noCache)
		{
			return [.. cache.Members.Values];
		}
		await UpdateMemberListAsync(groupUin);
		return [.. _memberCaches[groupUin].Members.Values];
	}

	public async Task<GroupMemberInfo?> GetGroupMemberInfoAsync(ulong groupUin, ulong memberUin, bool noCache = false)
	{
		if (_memberCaches.TryGetValue(groupUin, out var cache)
			&& !cache.RequiresUpdate
			&& !noCache
			&& cache.Members.TryGetValue(memberUin, out var info))
		{
			return info;
		}
		await UpdateMemberListAsync(groupUin);
		return _memberCaches[groupUin].Members.TryGetValue(memberUin, out info) ?
			info : null;
	}

	public async Task<string> ResolveGroupMemberDisplayNameAsync(ulong groupUin, ulong memberUin, bool noCache = false)
	{
		var friend = await friendCache.GetFriendInfoAsync(memberUin, noCache);
		if (friend is not null
			&& !string.IsNullOrEmpty(friend.Remark))
		{
			return friend.Remark; // 优先备注名
		}

		if (await GetGroupMemberInfoAsync(groupUin, memberUin, noCache) is { } member
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

	private readonly ConcurrentDictionary<ulong, Task<GroupMessageEntry?>> _latestMessages = [];

	private async Task<GroupMessageEntry?> GetLatestMessageEntryInternalAsync(ulong group)
	{
		try
		{
			if (groupMessageDatabase.Latest(group) is { } result)
			{
				return result;
			}

			if (adapterProvider.Adapter is not { } adapter)
			{
				throw new InvalidOperationException("Adapter is not available.");
			}

			// TODO: 从服务器拉取最新消息

			return null;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to get latest message of group {GroupUin}.", group);
			return null;
		}
	}

	public Task<GroupMessageEntry?> GetLatestMessageEntryAsync(ulong group)
		=> _latestMessages.GetOrAdd(group, GetLatestMessageEntryInternalAsync);

	#endregion

	#region 时间

	private readonly ConcurrentDictionary<ulong, Task<string>> _latestMessageTimes = [];

	private async Task<string> GetLatestMessageTimeInternalAsync(ulong group)
	{
		if (await GetLatestMessageEntryAsync(group) is not { } entry)
		{
			return string.Empty;
		}

		return entry.Time.ToLocalTime().ToString("HH:mm");
	}

	public Task<string> GetLatestMessageTimeAsync(ulong group)
		=> _latestMessageTimes.GetOrAdd(group, GetLatestMessageTimeInternalAsync);

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
		if (await GetLatestMessageEntryAsync(group) is not { } entry)
		{
			return string.Empty;
		}

		return await GenerateMessagePreviewAsync(group, entry);
	}

	public Task<string> GetLatestMessagePreviewAsync(ulong group)
		=> _latestMessagePreviews.GetOrAdd(group, GetLatestMessagePreviewInternalAsync);

	#endregion

	public void Adapter_OnGroupMessage(object? sender, GroupMessageEventArgs e)
	{
		_latestMessages[e.GroupUin] = Task.FromResult<GroupMessageEntry?>(e);
		_latestMessageTimes.TryRemove(e.GroupUin, out _);
		_latestMessagePreviews.TryRemove(e.GroupUin, out _);
	}

	#endregion
}
