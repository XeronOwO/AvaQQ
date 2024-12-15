namespace AvaQQ.Core.Messages;

/// <summary>
/// 节点片段
/// </summary>
public class NodeSegment : Segment
{
	/// <summary>
	/// QQ 号
	/// </summary>
	public ulong Uin { get; set; }

	/// <summary>
	/// 显示名称
	/// </summary>
	public string DisplayName { get; set; } = string.Empty;

	/// <summary>
	/// 内容
	/// </summary>
	public Message Content { get; set; } = [];
}
