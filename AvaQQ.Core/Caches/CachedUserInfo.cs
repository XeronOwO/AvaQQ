using AvaQQ.Core.Databases;

namespace AvaQQ.Core.Caches;

/// <summary>
/// 缓存的用户信息
/// </summary>
public class CachedUserInfo
{
	/// <summary>
	/// QQ 号
	/// </summary>
	public ulong Uin { get; internal set; }

	/// <summary>
	/// 昵称
	/// </summary>
	public string Nickname { get; internal set; } = string.Empty;

	/// <summary>
	/// 备注
	/// </summary>
	public string? Remark { get; internal set; }

	/// <summary>
	/// 是否是好友（通过在线数据获取）
	/// </summary>
	public bool IsFriend { get; internal set; }

	/// <summary/>
	public static implicit operator RecordedUserInfo(CachedUserInfo info)
		=> new(info.Uin, info.Nickname, info.Remark);
}
