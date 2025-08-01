namespace AvaQQ.Core.Entities.Messages;

/// <summary>
/// 段落接口
/// </summary>
public interface ISegment
{
	/// <summary>
	/// 群号<br/>非群消息时为 null
	/// </summary>
	public ulong? GroupUin { get; set; }

	/// <summary>
	/// 发送者 QQ 号
	/// </summary>
	public ulong SenderUin { get; set; }

	/// <summary>
	/// 消息序号
	/// </summary>
	public ulong Sequence { get; set; }

	/// <summary>
	/// 在本消息中的索引
	/// </summary>
	public uint Index { get; set; }

	/// <summary>
	/// 消息
	/// </summary>
	public Message Message { get; set; }
}
