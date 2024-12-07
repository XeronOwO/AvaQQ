using System.Threading.Tasks;

namespace AvaQQ.SDK.Adapters;

/// <summary>
/// 好友缓存
/// </summary>
public interface IFriendCache
{
	/// <summary>
	/// 获取所有好友的简略信息
	/// </summary>
	/// <param name="noCache">是否启用缓存</param>
	Task<FriendInfo[]> GetAllFriendInfosAsync(bool noCache = false);

	/// <summary>
	/// 获取好友的简略信息
	/// </summary>
	/// <param name="uin">好友 QQ 号</param>
	/// <param name="noCache">是否启用缓存</param>
	Task<FriendInfo?> GetFriendInfoAsync(ulong uin, bool noCache = false);
}
