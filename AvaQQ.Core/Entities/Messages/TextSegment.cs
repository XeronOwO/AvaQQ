using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaQQ.Core.Entities.Messages;

/// <summary>
/// 文本段落
/// </summary>
[Table("text_segment")]
[PrimaryKey(nameof(GroupUin), nameof(SenderUin), nameof(Sequence), nameof(Index))]
[Index(nameof(GroupUin))]
[Index(nameof(SenderUin))]
[Index(nameof(Sequence))]
[Index(nameof(Index))]
[Index(nameof(Text))]
public class TextSegment : ISegment
{
	/// <inheritdoc/>
	[Column]
	public ulong? GroupUin { get; set; }

	/// <inheritdoc/>
	[Column]
	[Required]
	public ulong SenderUin { get; set; }

	/// <inheritdoc/>
	[Column]
	[Required]
	public ulong Sequence { get; set; }

	/// <inheritdoc/>
	[Column]
	[Required]
	public uint Index { get; set; }

	/// <inheritdoc/>
	public virtual Message Message { get; set; } = null!;

	/// <summary>
	/// 文本内容
	/// </summary>
	[Column]
	[Required]
	public string Text { get; set; } = null!;
}
