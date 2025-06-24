using AvaQQ.Core.Databases;

namespace AvaQQ.Core.Caches;

/// <summary>
/// 缓存的群聊信息
/// </summary>
public class CachedGroupInfo
{
	/// <summary>
	/// 群号
	/// </summary>
	public ulong Uin { get; internal set; }

	/// <summary>
	/// 群名
	/// </summary>
	public string Name { get; internal set; } = string.Empty;

	/// <summary>
	/// 备注
	/// </summary>
	public string? Remark { get; internal set; }

	/// <summary>
	/// 是否现在仍然在该群（通过在线数据获取）
	/// </summary>
	public bool IsStillIn { get; internal set; }

	/// <summary/>
	public static implicit operator RecordedGroupInfo(CachedGroupInfo info)
		=> new(info.Uin, info.Name, info.Remark);
}
