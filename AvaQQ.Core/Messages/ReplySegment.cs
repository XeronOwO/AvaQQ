namespace AvaQQ.Core.Messages;

/// <summary>
/// 回复消息段
/// </summary>
/// <param name="Sequence">消息序号</param>
public record class ReplySegment(ulong Sequence) : ISegment
{
	/// <inheritdoc/>
	public string Preview => string.Empty;
}
