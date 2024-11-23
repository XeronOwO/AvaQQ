using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaQQ.SDK.Adapters;

/// <summary>
/// 协议适配器
/// </summary>
public interface IAdapter : IDisposable
{
	/// <summary>
	/// QQ 号<br/>在登录后就能获取，不会变动，因此不需要异步方法。
	/// </summary>
	long Uin { get; }

	/// <summary>
	/// 获取用户昵称
	/// </summary>
	Task<string> GetNicknameAsync();

	/// <summary>
	/// 获取好友列表
	/// </summary>
	Task<IEnumerable<BriefFriendInfo>> GetFriendListAsync();
}
