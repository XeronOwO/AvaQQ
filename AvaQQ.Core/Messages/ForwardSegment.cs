namespace AvaQQ.Core.Messages;

/// <summary>
/// 转发片段
/// </summary>
/// <param name="ResId">转发消息 ID</param>
public record class ForwardSegment(string ResId) : ISegment
{
	/// <inheritdoc/>
	public string Preview => string.Empty;
}
