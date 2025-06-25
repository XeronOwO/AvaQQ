using AvaQQ.Core.Adapters;
using AvaQQ.Core.Caches;
using AvaQQ.Core.Databases;
using AvaQQ.Core.Messages;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.Loader;

namespace AvaQQ.Core.Events;

/// <summary>
/// 事件车站
/// </summary>
public class EventStation
{
	private readonly ILogger<EventStation> _logger;

	/// <summary>
	/// 创建事件车站
	/// </summary>
	/// <param name="serviceProvider">服务提供者</param>
	/// <param name="logger">日志记录器</param>
	public EventStation(IServiceProvider serviceProvider, ILogger<EventStation> logger)
	{
		_logger = logger;

		OnUserAvatarFetched = new(serviceProvider, nameof(OnUserAvatarFetched));
		OnGroupAvatarFetched = new(serviceProvider, nameof(OnGroupAvatarFetched));
		OnUserAvatarChanged = new(serviceProvider, nameof(OnUserAvatarChanged));
		OnGroupAvatarChanged = new(serviceProvider, nameof(OnGroupAvatarChanged));
		OnRecordedUsersLoaded = new(serviceProvider, nameof(OnRecordedUsersLoaded));
		OnRecordedGroupsLoaded = new(serviceProvider, nameof(OnRecordedGroupsLoaded));
		OnFriendsFetched = new(serviceProvider, nameof(OnFriendsFetched));
		OnJoinedGroupsFetched = new(serviceProvider, nameof(OnJoinedGroupsFetched));
		OnNewUserCached = new(serviceProvider, nameof(OnNewUserCached));
		OnNewGroupCached = new(serviceProvider, nameof(OnNewGroupCached));
		OnUserNicknameChanged = new(serviceProvider, nameof(OnUserNicknameChanged));
		OnGroupNameChanged = new(serviceProvider, nameof(OnGroupNameChanged));
		OnUserRemarkChanged = new(serviceProvider, nameof(OnUserRemarkChanged));
		OnGroupRemarkChanged = new(serviceProvider, nameof(OnGroupRemarkChanged));
		OnFriendCacheAdded = new(serviceProvider, nameof(OnFriendCacheAdded));
		OnGroupCacheAdded = new(serviceProvider, nameof(OnGroupCacheAdded));
		OnFriendCacheRemoved = new(serviceProvider, nameof(OnFriendCacheRemoved));
		OnGroupCacheRemoved = new(serviceProvider, nameof(OnGroupCacheRemoved));
		OnUserFetched = new(serviceProvider, nameof(OnUserFetched));
		OnJoinedGroupFetched = new(serviceProvider, nameof(OnJoinedGroupFetched));
		OnGroupMessage = new(serviceProvider, nameof(OnGroupMessage));

		AssemblyLoadContext.Default.Unloading += _ =>
		{
			CheckSubscriptionsOnExit();
		};
	}

	/// <summary>
	/// 当从服务器获取到用户头像时
	/// </summary>
	public EventBus<AvatarId, byte[]?> OnUserAvatarFetched { get; }

	/// <summary>
	/// 当从服务器获取到群头像时
	/// </summary>
	public EventBus<AvatarId, byte[]?> OnGroupAvatarFetched { get; }

	/// <summary>
	/// 当用户头像改变时<br/>
	/// 是 <see cref="OnUserAvatarFetched"/> 的衍生事件
	/// </summary>
	public EventBus<AvatarId, AvatarChangedInfo> OnUserAvatarChanged { get; }

	/// <summary>
	/// 当群头像改变时<br/>
	/// 是 <see cref="OnGroupAvatarFetched"/> 的衍生事件
	/// </summary>
	public EventBus<AvatarId, AvatarChangedInfo> OnGroupAvatarChanged { get; }

	/// <summary>
	/// 当从本地数据库中加载用户信息时
	/// </summary>
	public EventBus<RecordedUserInfo[]> OnRecordedUsersLoaded { get; }

	/// <summary>
	/// 当从本地数据库中加载群信息时
	/// </summary>
	public EventBus<RecordedGroupInfo[]> OnRecordedGroupsLoaded { get; }

	/// <summary>
	/// 当从服务器获取好友列表时触发
	/// </summary>
	public EventBus<AdaptedUserInfo[]> OnFriendsFetched { get; }

	/// <summary>
	/// 当从服务器获取群列表时触发
	/// </summary>
	public EventBus<AdaptedGroupInfo[]> OnJoinedGroupsFetched { get; }

	/// <summary>
	/// 当缓存新的用户时触发<br/>
	/// 是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedUserInfo> OnNewUserCached { get; }

	/// <summary>
	/// 当缓存新的群时触发<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedGroupInfo> OnNewGroupCached { get; }

	/// <summary>
	/// 当用户昵称改变时触发<br/>
	/// 是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<UserNicknameChangedInfo> OnUserNicknameChanged { get; }

	/// <summary>
	/// 当群名称改变时触发<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<GroupNameChangedInfo> OnGroupNameChanged { get; }

	/// <summary>
	/// 当用户备注改变时触发<br/>
	/// 是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<UserRemarkChangedInfo> OnUserRemarkChanged { get; }

	/// <summary>
	/// 当群备注改变时触发<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<GroupRemarkChangedInfo> OnGroupRemarkChanged { get; }

	/// <summary>
	///	当添加好友缓存时<br/>
	///	是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedUserInfo> OnFriendCacheAdded { get; }

	/// <summary>
	/// 当添加群缓存时<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedGroupInfo> OnGroupCacheAdded { get; }

	/// <summary>
	/// 当移除好友缓存时<br/>
	/// 是 <see cref="OnFriendsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedUserInfo> OnFriendCacheRemoved { get; }

	/// <summary>
	/// 当移除群缓存时<br/>
	/// 是 <see cref="OnJoinedGroupsFetched"/> 的衍生事件
	/// </summary>
	public EventBus<CachedGroupInfo> OnGroupCacheRemoved { get; }

	/// <summary>
	/// 当从服务器获取用户信息时触发
	/// </summary>
	public EventBus<UinId, AdaptedUserInfo?> OnUserFetched { get; }

	/// <summary>
	/// 当从服务器获取加入的群后触发
	/// </summary>
	public EventBus<UinId, AdaptedGroupInfo?> OnJoinedGroupFetched { get; }

	/// <summary>
	/// 当有群消息时触发
	/// </summary>
	public EventBus<Message> OnGroupMessage { get; }

	private void CheckSubscriptionsOnExit()
	{
		_logger.LogDebug("Checking event subscriptions on exit...");

		var properties = GetType().GetProperties();
		var result = true;
		foreach (var property in properties)
		{
			if (property.PropertyType.IsAssignableTo(typeof(IEventBus)))
			{
				var eventBus = (IEventBus)property.GetValue(this)!;
				if (!eventBus.CheckSubscriptionsOnExit())
				{
					result = false;
				}
			}
		}

		Debug.Assert(result, "There are still some subscriptions left when exiting the application. This may cause memory leaks or other issues." +
			" Please check the event handlers at \"logs/latest.log\" and ensure they are properly unsubscribed when no longer needed." +
			" If you do unsubscriptions in dtor, it may cause false positives, because the dtor may not be called before the application exits.");
	}
}
