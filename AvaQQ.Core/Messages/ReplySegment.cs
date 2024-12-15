namespace AvaQQ.Core.Messages;

/// <summary>
/// 回复消息段
/// </summary>
public class ReplySegment : Segment
{
	/// <summary>
	/// 消息 ID
	/// </summary>
	public ulong MessageId { get; set; }
}
