namespace AvaQQ.Core.Messages;

/// <summary>
/// @某人片段
/// </summary>
public class AtSegment : Segment
{
	/// <summary>
	/// QQ 号<br/>
	/// 0 表示 @全体成员
	/// </summary>
	public ulong Uin { get; set; }
}
