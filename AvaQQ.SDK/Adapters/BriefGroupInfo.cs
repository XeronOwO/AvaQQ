namespace AvaQQ.SDK.Adapters;

/// <summary>
/// 群聊简略信息
/// </summary>
public class BriefGroupInfo
{
	/// <summary>
	/// 群号
	/// </summary>
	public ulong Uin { get; set; }

	/// <summary>
	/// 群名
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// 备注
	/// </summary>
	public string Remark { get; set; } = string.Empty;
}
