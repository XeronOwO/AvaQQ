namespace AvaQQ.Core.Messages;

/// <summary>
/// JSON 段
/// </summary>
/// <param name="Data">数据</param>
public record class JsonSegment(string Data) : ISegment
{
	/// <inheritdoc/>
	public string Preview => string.Empty;
}
