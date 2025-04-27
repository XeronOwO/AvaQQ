namespace AvaQQ.Core.Messages;

/// <summary>
/// @某人片段
/// </summary>
/// <param name="Uin">QQ 号</param>
/// <param name="Name">显示名称</param>
public record class AtSegment(ulong Uin, string Name) : ISegment
{
	/// <inheritdoc/>
	public string Preview => $"@{Name} ";
}
