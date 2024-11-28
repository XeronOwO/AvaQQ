using AvaQQ.SDK.Adapters;
using System.Threading.Tasks;

namespace AvaQQ.SDK;

/// <summary>
/// 用户管理器
/// </summary>
public interface IUserManager
{
	/// <summary>
	/// 获取所有好友的简略信息
	/// </summary>
	Task<BriefFriendInfo[]> GetAllFriendInfosAsync();

	/// <summary>
	/// 获取好友的简略信息
	/// </summary>
	/// <param name="uin">好友 QQ 号</param>
	Task<BriefFriendInfo?> GetFriendInfoAsync(ulong uin);
}
