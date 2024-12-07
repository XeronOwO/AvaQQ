using System.Threading.Tasks;

namespace AvaQQ.SDK.Adapters;

/// <summary>
/// 用户缓存
/// </summary>
public interface IUserCache
{
	/// <summary>
	/// 获取用户信息
	/// </summary>
	/// <param name="uin">QQ 号</param>
	/// <param name="noCache">是否启用缓存</param>
	Task<UserInfo?> GetUserInfoAsync(ulong uin, bool noCache = false);
}
