using AvaQQ.Core.Events;

namespace AvaQQ.Core.Caches;

/// <summary>
/// 群缓存
/// </summary>
public interface IGroupCache : IDisposable
{
	/// <summary>
	/// 获取所有群聊信息，包括已加入的、已退出的、搜索缓存的等<br/>
	/// 如果触发更新，请订阅 <see cref="EventStation.GetAllJoinedGroups"/> 事件
	/// </summary>
	/// <param name="predicate">描述</param>
	/// <param name="forceUpdate">强制更新</param>
	CachedGroupInfo[] GetGroups(Func<CachedGroupInfo, bool> predicate, bool forceUpdate = false);

	/// <summary>
	/// 获取加入的群聊信息<br/>
	/// 如果触发更新，请订阅 <see cref="EventStation.GetJoinedGroup"/> 事件
	/// </summary>
	/// <param name="uin">群号</param>
	/// <param name="forceUpdate">强制更新</param>
	CachedGroupInfo? GetJoinedGroup(ulong uin, bool forceUpdate = false);
}
