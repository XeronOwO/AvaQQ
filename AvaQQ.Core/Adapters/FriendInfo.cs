namespace AvaQQ.Core.Adapters;

/// <summary>
/// 好友信息
/// </summary>
public class FriendInfo
{
	/// <summary>
	/// QQ 号
	/// </summary>
	public ulong Uin { get; set; }

	/// <summary>
	/// 昵称
	/// </summary>
	public string Nickname { get; set; } = string.Empty;

	/// <summary>
	/// 备注
	/// </summary>
	public string Remark { get; set; } = string.Empty;
}
