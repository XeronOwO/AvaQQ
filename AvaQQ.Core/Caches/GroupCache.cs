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
	IUserCache userCache
	) : IGroupCache
{
	#region 群信息

	private readonly ConcurrentDictionary<ulong, GroupInfo> _infos = [];

	private DateTime _lastUpdateTime = DateTime.MinValue;

	private bool BriefGroupRequiresUpdate
		=> DateTime.Now - _lastUpdateTime > Config.Instance.GroupUpdateInterval;

	private async Task UpdateGroupListAsync()
	{
		if (adapterProvider.Adapter is not { } adapter)
		{
			return;
		}

		try
		{
			var friendList = await adapter.GetGroupListAsync();
			_infos.Clear();
			foreach (var friend in friendList)
			{
				_infos[friend.Uin] = friend;
			}

			_lastUpdateTime = DateTime.Now;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to update group list.");
		}
	}

	public async Task<GroupInfo?> GetGroupInfoAsync(ulong uin, bool noCache = false)
	{
		if (BriefGroupRequiresUpdate
			|| noCache)
		{
			await UpdateGroupListAsync();
		}

		return _infos.TryGetValue(uin, out var info) ? info : null;
	}

	public async Task<GroupInfo[]> GetAllGroupInfosAsync(bool noCache = false)
	{
		if (BriefGroupRequiresUpdate
			|| noCache)
		{
			await UpdateGroupListAsync();
		}

		return [.. _infos.Values];
	}

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

	public Task<string> GenerateMessagePreviewAsync(ulong groupUin, GroupMessageEntry entry)
		=> GenerateMessagePreviewAsync(groupUin, entry.SenderUin, entry.Message);

	public async Task<string> GenerateMessagePreviewAsync(ulong groupUin, ulong memberUin, Message message)
	{
		var sb = new StringBuilder();

		if (adapterProvider.Adapter is { } adapter
			&& adapter.Uin != memberUin)
		{
			sb.Append(await ResolveGroupMemberDisplayNameAsync(groupUin, memberUin));
			sb.Append(": ");
		}

		foreach (var segment in message)
		{
			switch (segment)
			{
				case AtSegment:
					sb.Append('@');
					sb.Append(await ResolveGroupMemberDisplayNameAsync(groupUin, memberUin));
					break;
				case FaceSegment:
					sb.Append(SR.FaceSegmentPreview);
					break;
				case ForwardSegment:
					sb.Append(SR.ForwardSegmentPreview);
					break;
				case ImageSegment image:
					switch (image.SubType)
					{
						case 0:
							sb.Append(SR.ImageSegmentPreview0);
							break;
						case 1:
							sb.Append(SR.ImageSegmentPreview1);
							break;
						default:
							logger.LogWarning("Unsupported image segment sub type {SubType}.", image.SubType);
							break;
					}
					break;
				case ReplySegment:
					break;
				case TextSegment text:
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

	#endregion
}
