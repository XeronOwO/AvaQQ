using Avalonia.Media.Imaging;
using AvaQQ.Core.Adapters;
using AvaQQ.Core.Caches;
using AvaQQ.Core.Databases;

namespace AvaQQ.Core.Events;

/// <summary>
/// 事件车站
/// </summary>
public class EventStation
{
	#region 用户

	/// <summary>
	/// 用户头像
	/// </summary>
	public EventBus<AvatarCacheId, Bitmap?> UserAvatar { get; set; } = new();

	/// <summary>
	/// 当从数据库中加载所有用户信息时触发
	/// </summary>
	public EventBus<CommonEventId, RecordedUserInfo[]> GetAllRecordedUsers { get; set; } = new();

	/// <summary>
	/// 获取所有好友
	/// </summary>
	public EventBus<CommonEventId, AdaptedUserInfo[]> GetAllFriends { get; set; } = new();

	/// <summary>
	/// 缓存的获取所有好友
	/// </summary>
	public EventBus<CommonEventId, CachedUserInfo[]> CachedGetAllFriends { get; set; } = new();

	/// <summary>
	/// 获取用户信息
	/// </summary>
	public EventBus<UinId, AdaptedUserInfo?> GetUser { get; set; } = new();

	#endregion

	#region 群

	/// <summary>
	/// 群头像
	/// </summary>
	public EventBus<AvatarCacheId, Bitmap?> GroupAvatar { get; set; } = new();

	/// <summary>
	/// 当从数据库中加载所有群信息时触发
	/// </summary>
	public EventBus<CommonEventId, RecordedGroupInfo[]> GetAllRecordedGroups { get; set; } = new();

	/// <summary>
	/// 获取加入的所有群聊
	/// </summary>
	public EventBus<CommonEventId, AdaptedGroupInfo[]> GetAllJoinedGroups { get; set; } = new();

	/// <summary>
	/// 缓存的加入的所有群聊更新<br/>
	/// 在 <see cref="GetAllJoinedGroups"/> 事件期间触发
	/// </summary>
	public EventBus<CommonEventId, CachedGroupInfo[]> CachedGetAllJoinedGroups { get; set; } = new();

	/// <summary>
	/// 获取加入的群聊信息
	/// </summary>
	public EventBus<UinId, AdaptedGroupInfo?> GetJoinedGroup { get; set; } = new();

	/// <summary>
	/// 缓存的加入的群聊更新<br/>
	/// 在 <see cref="GetJoinedGroup"/> 事件期间触发
	/// </summary>
	public EventBus<UinId, CachedGroupInfo?> CachedGetJoinedGroup { get; set; } = new();

	#endregion
}
