using AvaQQ.Core.Adapters;
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
	public required ulong Uin { get; set; }

	/// <summary>
	/// 群名
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// 备注
	/// </summary>
	public required string? Remark { get; set; }

	/// <summary>
	/// 是否有本地数据（通过数据库获取）
	/// </summary>
	public bool HasLocalData { get; set; }

	/// <summary>
	/// 是否现在仍然在该群（通过在线数据获取）
	/// </summary>
	public bool IsStillIn { get; set; }

	/// <inheritdoc/>
	public static implicit operator CachedGroupInfo(RecordedGroupInfo info)
		=> new()
		{
			Uin = info.Uin,
			Name = info.Name,
			Remark = info.Remark,
		};

	/// <inheritdoc/>
	public static implicit operator RecordedGroupInfo(CachedGroupInfo info)
		=> new(info.Uin, info.Name, info.Remark);

	/// <inheritdoc/>
	public static implicit operator CachedGroupInfo(AdaptedGroupInfo info)
		=> new()
		{
			Uin = info.Uin,
			Name = info.Name,
			Remark = info.Remark,
		};

	/// <summary>
	/// 更新缓存的群聊信息
	/// </summary>
	/// <param name="info">信息</param>
	public void Update(AdaptedGroupInfo? info)
	{
		if (info == null)
		{
			return;
		}

		Uin = info.Value.Uin;
		Name = info.Value.Name;
		Remark = info.Value.Remark;
	}
}
