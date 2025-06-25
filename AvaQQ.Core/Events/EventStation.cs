using AvaQQ.Core.Adapters;
using AvaQQ.Core.Caches;
using AvaQQ.Core.Databases;
using AvaQQ.Core.Messages;

namespace AvaQQ.Core.Events;

/// <summary>
/// 事件车站
/// </summary>
public class EventStation(IServiceProvider serviceProvider)
{
	/// <summary>
	/// 当从服务器获取到用户头像时
	/// </summary>
	public EventBus<AvatarId, byte[]?> OnUserAvatarFetched { get; } = new(serviceProvider, nameof(OnUserAvatarFetched));

	/// <summary>
	/// 当从服务器获取到群头像时
	/// </summary>
	public EventBus<AvatarId, byte[]?> OnGroupAvatarFetched { get; } = new(serviceProvider, nameof(OnGroupAvatarFetched));

	/// <summary>
	/// 当用户头像改变时<br/>
	/// 是 <see cref="OnUserAvatarFetched"/> 的衍生事件
	/// </summary>
	public EventBus<AvatarId, AvatarChangedInfo> OnUserAvatarChanged { get; } = new(serviceProvider, nameof(OnUserAvatarChanged));

	/// <summary>
	/// 当群头像改变时<br/>
	/// 是 <see cref="OnGroupAvatarFetched"/> 的衍生事件
	/// </summary>
	public EventBus<AvatarId, AvatarChangedInfo> OnGroupAvatarChanged { get; } = new(serviceProvider, nameof(OnGroupAvatarChanged));

	/// <summary>
	/// 当从本地数据库中加载用户信息时
	/// </summary>
	public EventBus<RecordedUserInfo[]> OnRecordedUsersLoaded { get; } = new(serviceProvider, nameof(OnRecordedUsersLoaded));

	/// <summary>
	/// 当从本地数据库中加载群信息时
	/// </summary>
	public EventBus<RecordedGroupInfo[]> OnRecordedGroupsLoaded { get; } = new(serviceProvider, nameof(OnRecordedGroupsLoaded));

	/// <summary>
	/// 当从服务器获取好友列表时触发
	/// </summary>
	public EventBus<AdaptedUserInfo[]> OnFriendsFetched { get; } = new(serviceProvider, nameof(OnFriendsFetched));

	/// <summary>
	/// 当从服务器获取群列表时触发
	/// </summary>
	public EventBus<AdaptedGroupInfo[]> OnJoinedGroupsFetched { get; } = new(serviceProvider, nameof(OnJoinedGroupsFetched));

	/// <summary>
	/// 当缓存新的用户时触发<br/>
	/// 是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedUserInfo> OnNewUserCached { get; } = new(serviceProvider, nameof(OnNewUserCached));

	/// <summary>
	/// 当缓存新的群时触发<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedGroupInfo> OnNewGroupCached { get; } = new(serviceProvider, nameof(OnNewGroupCached));

	/// <summary>
	/// 当用户昵称改变时触发<br/>
	/// 是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<UserNicknameChangedInfo> OnUserNicknameChanged { get; } = new(serviceProvider, nameof(OnUserNicknameChanged));

	/// <summary>
	/// 当群名称改变时触发<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<GroupNameChangedInfo> OnGroupNameChanged { get; } = new(serviceProvider, nameof(OnGroupNameChanged));

	/// <summary>
	/// 当用户备注改变时触发<br/>
	/// 是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<UserRemarkChangedInfo> OnUserRemarkChanged { get; } = new(serviceProvider, nameof(OnUserRemarkChanged));

	/// <summary>
	/// 当群备注改变时触发<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<GroupRemarkChangedInfo> OnGroupRemarkChanged { get; } = new(serviceProvider, nameof(OnGroupRemarkChanged));

	/// <summary>
	///	当添加好友缓存时<br/>
	///	是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedUserInfo> OnFriendCacheAdded { get; } = new(serviceProvider, nameof(OnFriendCacheAdded));

	/// <summary>
	/// 当添加群缓存时<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedGroupInfo> OnGroupCacheAdded { get; } = new(serviceProvider, nameof(OnGroupCacheAdded));

	/// <summary>
	/// 当移除好友缓存时<br/>
	/// 是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedUserInfo> OnFriendCacheRemoved { get; } = new(serviceProvider, nameof(OnFriendCacheRemoved));

	/// <summary>
	/// 当移除群缓存时<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedGroupInfo> OnGroupCacheRemoved { get; } = new(serviceProvider, nameof(OnGroupCacheRemoved));

	/// <summary>
	/// 当从服务器获取用户信息时触发
	/// </summary>
	public EventBus<UinId, AdaptedUserInfo?> OnUserFetched { get; } = new(serviceProvider, nameof(OnUserFetched));

	/// <summary>
	/// 当从服务器获取加入的群后触发
	/// </summary>
	public EventBus<UinId, AdaptedGroupInfo?> OnJoinedGroupFetched { get; } = new(serviceProvider, nameof(OnJoinedGroupFetched));

	/// <summary>
	/// 当有群消息时触发
	/// </summary>
	public EventBus<Message> OnGroupMessage { get; } = new(serviceProvider, nameof(OnGroupMessage));
}
