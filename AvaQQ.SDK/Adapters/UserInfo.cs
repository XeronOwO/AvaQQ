namespace AvaQQ.SDK.Adapters;

/// <summary>
/// 用户信息
/// </summary>
public class UserInfo
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
	/// 性别
	/// </summary>
	public SexType Sex { get; set; } = SexType.Unknown;

	/// <summary>
	/// 年龄
	/// </summary>
	public int Age { get; set; }
}
