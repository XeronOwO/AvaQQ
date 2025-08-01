
namespace AvaQQ.SDK.Entities;

public interface IMessage
{
	ulong? GroupUin { get; }

	ulong SenderUin { get; }

	ulong Sequence { get; }

	DateTimeOffset Time { get; }

	IEnumerable<ITextSegment> TextSegments { get; }

	IEnumerable<ISegment?> Segments { get; }
}
