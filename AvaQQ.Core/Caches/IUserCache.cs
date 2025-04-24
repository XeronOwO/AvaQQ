using AvaQQ.Core.Events;

namespace AvaQQ.Core.Caches;

/// <summary>
/// 用户缓存
/// </summary>
public interface IUserCache : IDisposable
{
	/// <summary>
	/// 获取所有用户信息，包括好友、陌生人、搜索缓存的等<br/>
	/// 如果触发更新，请订阅 <see cref="EventStation.GetAllFriends"/> 事件
	/// </summary>
	/// <param name="predicate">描述</param>
	/// <param name="forceUpdate">强制更新</param>
	CachedUserInfo[] GetUsers(Func<CachedUserInfo, bool> predicate, bool forceUpdate = false);

	/// <summary>
	/// 获取用户信息<br/>
	/// 如果触发更新，请订阅 <see cref="EventStation.GetUser"/> 事件
	/// </summary>
	/// <param name="uin">QQ 号</param>
	/// <param name="forceUpdate">强制更新</param>
	CachedUserInfo? GetUser(ulong uin, bool forceUpdate = false);
}
