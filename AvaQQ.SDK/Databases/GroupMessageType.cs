namespace AvaQQ.SDK.Databases;

/// <summary>
/// 群消息类型
/// </summary>
public enum GroupMessageType
{
	/// <summary>
	/// 正常消息
	/// </summary>
	Normal,

	/// <summary>
	/// 匿名消息
	/// </summary>
	Anonymous,

	/// <summary>
	/// 系统提示（如「管理员已禁止群内匿名聊天」）
	/// </summary>
	Notice,
}
