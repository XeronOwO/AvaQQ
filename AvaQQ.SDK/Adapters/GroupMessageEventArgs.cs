using AvaQQ.SDK.Databases;

namespace AvaQQ.SDK.Adapters;

/// <summary>
/// 群消息事件参数
/// </summary>
public class GroupMessageEventArgs : GroupMessageEntry
{
	/// <summary>
	/// 发送者的昵称
	/// </summary>
	public string SenderNickname { get; set; } = string.Empty;

	/// <summary>
	/// 发送者的群内昵称
	/// </summary>
	public string SenderGroupNickname { get; set; } = string.Empty;

	/// <summary>
	/// 自己设置的发送者的备注
	/// </summary>
	public string SenderRemark { get; set; } = string.Empty;

	/// <summary>
	/// 发送者的群等级
	/// </summary>
	public int SenderLevel { get; set; }

	/// <summary>
	/// 发送者的群角色
	/// </summary>
	public GroupRoleType SenderRole { get; set; }

	/// <summary>
	/// 发送者的专属群头衔
	/// </summary>
	public string SpecificTitle { get; set; } = string.Empty;
}
