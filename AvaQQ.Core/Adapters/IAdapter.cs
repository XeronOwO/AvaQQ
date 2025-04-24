namespace AvaQQ.Core.Adapters;

/// <summary>
/// 协议适配器
/// </summary>
public interface IAdapter : IDisposable
{
	/// <summary>
	/// QQ 号<br/>在登录后就能获取，不会变动，因此不需要异步方法。
	/// </summary>
	ulong Uin { get; }

	/// <summary>
	/// 获取用户信息
	/// </summary>
	/// <param name="uin">QQ 号</param>
	/// <param name="token">取消令牌</param>
	Task<AdaptedUserInfo?> GetUserAsync(ulong uin, CancellationToken token = default);

	/// <summary>
	/// 获取所有好友信息
	/// </summary>
	/// <param name="token">取消令牌</param>
	Task<AdaptedUserInfo[]> GetAllFriendsAsync(CancellationToken token = default);

	/// <summary>
	/// 获取所有加入的群聊
	/// </summary>
	/// <param name="token">取消令牌</param>
	Task<AdaptedGroupInfo[]> GetAllJoinedGroupsAsync(CancellationToken token = default);

	/// <summary>
	/// 获取加入的群聊信息
	/// </summary>
	/// <param name="uin">群号</param>
	/// <param name="token">取消令牌</param>
	Task<AdaptedGroupInfo?> GetJoinedGroupAsync(ulong uin, CancellationToken token = default);
}
