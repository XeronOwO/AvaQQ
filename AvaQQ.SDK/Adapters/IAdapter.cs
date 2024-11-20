using System;
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
	/// <returns>昵称</returns>
	Task<string> GetNicknameAsync();
}
