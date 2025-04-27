namespace AvaQQ.Core.Messages;

/// <summary>
/// 文本片段
/// </summary>
/// <param name="Text">文本</param>
public record class TextSegment(string Text) : ISegment
{
	/// <inheritdoc/>
	public string Preview => Text;
}
