namespace AvaQQ.SDK.Messages;

/// <summary>
/// 转发片段
/// </summary>
public class ForwardSegment : Segment
{
	/// <summary>
	/// 转发消息 ID
	/// </summary>
	public string ResId { get; set; } = string.Empty;
}
