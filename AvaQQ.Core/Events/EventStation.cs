using AvaQQ.Core.Adapters;
using AvaQQ.Core.Caches;
using AvaQQ.Core.Databases;
using AvaQQ.Core.Messages;

namespace AvaQQ.Core.Events;

/// <summary>
/// 事件车站
/// </summary>
public class EventStation
{
	/// <summary>
	/// 当从服务器获取到用户头像时
	/// </summary>
	public EventBus<AvatarId, byte[]?> OnUserAvatarFetched { get; } = new();

	/// <summary>
	/// 当从服务器获取到群头像时
	/// </summary>
	public EventBus<AvatarId, byte[]?> OnGroupAvatarFetched { get; } = new();

	/// <summary>
	/// 当用户头像改变时<br/>
	/// 是 <see cref="OnUserAvatarFetched"/> 的衍生事件
	/// </summary>
	public EventBus<AvatarId, AvatarChangedInfo> OnUserAvatarChanged { get; } = new();

	/// <summary>
	/// 当群头像改变时<br/>
	/// 是 <see cref="OnGroupAvatarFetched"/> 的衍生事件
	/// </summary>
	public EventBus<AvatarId, AvatarChangedInfo> OnGroupAvatarChanged { get; } = new();

	/// <summary>
	/// 当从本地数据库中加载用户信息时
	/// </summary>
	public EventBus<RecordedUserInfo[]> OnRecordedUsersLoaded { get; } = new();

	/// <summary>
	/// 当从本地数据库中加载群信息时
	/// </summary>
	public EventBus<RecordedGroupInfo[]> OnRecordedGroupsLoaded { get; } = new();

	/// <summary>
	/// 当从服务器获取好友列表时触发
	/// </summary>
	public EventBus<AdaptedUserInfo[]> OnFriendsFetched { get; } = new();

	/// <summary>
	/// 当从服务器获取群列表时触发
	/// </summary>
	public EventBus<AdaptedGroupInfo[]> OnJoinedGroupsFetched { get; } = new();

	/// <summary>
	/// 当缓存新的用户时触发<br/>
	/// 是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedUserInfo> OnNewUserCached { get; } = new();

	/// <summary>
	/// 当缓存新的群时触发<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedGroupInfo> OnNewGroupCached { get; } = new();

	/// <summary>
	/// 当用户昵称改变时触发<br/>
	/// 是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<UserNicknameChangedInfo> OnUserNicknameChanged { get; } = new();

	/// <summary>
	/// 当群名称改变时触发<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<GroupNameChangedInfo> OnGroupNameChanged { get; } = new();

	/// <summary>
	/// 当用户备注改变时触发<br/>
	/// 是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<UserRemarkChangedInfo> OnUserRemarkChanged { get; } = new();

	/// <summary>
	/// 当群备注改变时触发<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<GroupRemarkChangedInfo> OnGroupRemarkChanged { get; } = new();

	/// <summary>
	///	当添加好友缓存时<br/>
	///	是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedUserInfo> OnFriendCacheAdded { get; } = new();

	/// <summary>
	/// 当添加群缓存时<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedGroupInfo> OnGroupCacheAdded { get; } = new();

	/// <summary>
	/// 当移除好友缓存时<br/>
	/// 是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedUserInfo> OnFriendCacheRemoved { get; } = new();

	/// <summary>
	/// 当移除群缓存时<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedGroupInfo> OnGroupCacheRemoved { get; } = new();

	/// <summary>
	/// 当从服务器获取用户信息时触发
	/// </summary>
	public EventBus<UinId, AdaptedUserInfo?> OnUserFetched { get; } = new();

	/// <summary>
	/// 当从服务器获取加入的群后触发
	/// </summary>
	public EventBus<UinId, AdaptedGroupInfo?> OnJoinedGroupFetched { get; } = new();

	/// <summary>
	/// 当有群消息时触发
	/// </summary>
	public EventBus<Message> OnGroupMessage { get; } = new();
}
