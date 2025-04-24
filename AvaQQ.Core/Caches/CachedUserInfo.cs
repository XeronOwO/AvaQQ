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
	public required ulong Uin { get; set; }

	/// <summary>
	/// 昵称
	/// </summary>
	public required string Nickname { get; set; }

	/// <summary>
	/// 备注
	/// </summary>
	public required string? Remark { get; set; }

	/// <summary>
	/// 是否有本地数据（通过数据库获取）
	/// </summary>
	public bool HasLocalData { get; set; }

	/// <summary>
	/// 是否是好友（通过在线数据获取）
	/// </summary>
	public bool IsFriend { get; set; }

	/// <summary/>
	public static implicit operator RecordedUserInfo(CachedUserInfo info)
		=> new(info.Uin, info.Nickname, info.Remark);
}
