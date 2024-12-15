namespace AvaQQ.Core.Messages;

/// <summary>
/// 文本片段
/// </summary>
public class TextSegment : Segment
{
	/// <summary>
	/// 文本
	/// </summary>
	public string Text { get; set; } = string.Empty;
}
