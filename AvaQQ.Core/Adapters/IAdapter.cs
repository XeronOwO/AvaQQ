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
	/// 获取用户昵称
	/// </summary>
	Task<string> GetNicknameAsync();

	/// <summary>
	/// 获取好友列表
	/// </summary>
	Task<IEnumerable<FriendInfo>> GetFriendListAsync();

	/// <summary>
	/// 获取群列表
	/// </summary>
	Task<IEnumerable<GroupInfo>> GetGroupListAsync();

	/// <summary>
	/// 获取用户信息
	/// </summary>
	/// <param name="uin">QQ 号</param>
	Task<UserInfo?> GetUserInfoAsync(ulong uin);

	/// <summary>
	/// 获取群成员列表
	/// </summary>
	/// <param name="groupUin">群号</param>
	Task<IEnumerable<GroupMemberInfo>> GetGroupMemberListAsync(ulong groupUin);

	/// <summary>
	/// 当收到群消息时触发
	/// </summary>
	event EventHandler<GroupMessageEventArgs>? OnGroupMessage;
}
