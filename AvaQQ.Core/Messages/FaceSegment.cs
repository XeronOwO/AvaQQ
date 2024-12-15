namespace AvaQQ.Core.Messages;

/// <summary>
/// 表情片段
/// </summary>
public class FaceSegment : Segment
{
	/// <summary>
	/// 表情 ID
	/// </summary>
	public ulong Id { get; set; }

	/// <summary>
	/// 是否是大表情
	/// </summary>
	public bool IsLarge { get; set; }
}
