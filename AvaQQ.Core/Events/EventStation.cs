using Avalonia.Media.Imaging;
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
	#region 用户

	/// <summary>
	/// 当通过 Url 获取用户头像后触发
	/// </summary>
	public EventBus<AvatarCacheId, Bitmap?> UserAvatar { get; set; } = new();

	/// <inheritdoc cref="CommonEventId.GetAllRecordedUsers"/>
	public EventBus<CommonEventId, RecordedUserInfo[]> GetAllRecordedUsers { get; set; } = new();

	/// <inheritdoc cref="CommonEventId.GetAllFriends"/>
	public EventBus<CommonEventId, AdaptedUserInfo[]> GetAllFriends { get; set; } = new();

	/// <inheritdoc cref="CommonEventId.CachedGetAllFriends"/>
	public EventBus<CommonEventId, CachedUserInfo[]> CachedGetAllFriends { get; set; } = new();

	/// <summary>
	/// 当从服务器获取用户信息后触发
	/// </summary>
	public EventBus<UinId, AdaptedUserInfo?> GetUser { get; set; } = new();

	#endregion

	#region 群

	/// <summary>
	/// 当通过 Url 获取群头像后触发
	/// </summary>
	public EventBus<AvatarCacheId, Bitmap?> GroupAvatar { get; set; } = new();

	/// <inheritdoc cref="CommonEventId.GetAllRecordedGroups"/>
	public EventBus<CommonEventId, RecordedGroupInfo[]> GetAllRecordedGroups { get; set; } = new();

	/// <inheritdoc cref="CommonEventId.GetAllJoinedGroups"/>
	public EventBus<CommonEventId, AdaptedGroupInfo[]> GetAllJoinedGroups { get; set; } = new();

	/// <inheritdoc cref="CommonEventId.CachedGetAllJoinedGroups"/>
	public EventBus<CommonEventId, CachedGroupInfo[]> CachedGetAllJoinedGroups { get; set; } = new();

	/// <summary>
	/// 当从服务器获取加入的群后触发
	/// </summary>
	public EventBus<UinId, AdaptedGroupInfo?> GetJoinedGroup { get; set; } = new();

	/// <summary>
	/// 当从服务器获取加入的群、并更新缓存后触发<br/>
	/// 在 <see cref="GetJoinedGroup"/> 期间触发
	/// </summary>
	public EventBus<UinId, CachedGroupInfo?> CachedGetJoinedGroup { get; set; } = new();

	/// <inheritdoc cref="CommonEventId.GroupMessage"/>
	public EventBus<CommonEventId, Message> GroupMessage { get; set; } = new();

	#endregion
}
