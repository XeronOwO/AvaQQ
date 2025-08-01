namespace AvaQQ.SDK.Entities;

public interface ISegment
{
	public ulong? GroupUin { get; }

	public ulong SenderUin { get; }

	public ulong Sequence { get; }

	public uint Index { get; }

	public IMessage Message { get; }
}
