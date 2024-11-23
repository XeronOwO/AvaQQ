namespace AvaQQ.SDK.Adapters;

/// <summary>
/// 好友简略信息
/// </summary>
public class BriefFriendInfo
{
	/// <summary>
	/// QQ 号
	/// </summary>
	public long Uin { get; set; }

	/// <summary>
	/// 昵称
	/// </summary>
	public string Nickname { get; set; } = string.Empty;

	/// <summary>
	/// 备注
	/// </summary>
	public string Remark { get; set; } = string.Empty;
}
