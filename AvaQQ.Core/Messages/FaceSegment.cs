namespace AvaQQ.Core.Messages;

/// <summary>
/// 表情片段
/// </summary>
/// <param name="Id">表情 ID</param>
/// <param name="IsLarge">是否是大表情</param>
public record class FaceSegment(ulong Id, bool IsLarge) : ISegment
{
	/// <inheritdoc/>
	public string Preview => string.Empty;
}
