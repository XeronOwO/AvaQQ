using AvaQQ.SDK.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaQQ.Entities;

[Table("text_segment")]
[PrimaryKey(nameof(GroupUin), nameof(SenderUin), nameof(Sequence), nameof(Index))]
[Index(nameof(GroupUin))]
[Index(nameof(SenderUin))]
[Index(nameof(Sequence))]
[Index(nameof(Index))]
[Index(nameof(Text))]
public class TextSegment : ITextSegment
{
	[Column]
	public ulong? GroupUin { get; set; }

	[Column]
	[Required]
	public ulong SenderUin { get; set; }

	[Column]
	[Required]
	public ulong Sequence { get; set; }

	[Column]
	[Required]
	public uint Index { get; set; }

	public virtual Message Message { get; set; } = null!;

	IMessage ISegment.Message => Message;

	[Column]
	[Required]
	public string Text { get; set; } = null!;
}
