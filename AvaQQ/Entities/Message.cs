using AvaQQ.SDK.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaQQ.Entities;

[Table("message")]
[PrimaryKey(nameof(GroupUin), nameof(SenderUin), nameof(Sequence))]
[Index(nameof(GroupUin))]
[Index(nameof(SenderUin))]
[Index(nameof(Sequence))]
[Index(nameof(Time))]
public class Message : IMessage
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
	public DateTimeOffset Time { get; set; }

	public virtual List<TextSegment> TextSegments { get; set; } = null!;

	IEnumerable<ITextSegment> IMessage.TextSegments => TextSegments;

	public IEnumerable<ISegment?> Segments
	{
		get
		{
			var segments = new List<ISegment?>();
			segments.AddRange(TextSegments);

			segments.Sort((s1, s2) => s1!.Index.CompareTo(s2!.Index));
			for (int i = 0; i < segments.Count; i++)
			{
				while (i < segments[i]!.Index)
				{
					segments.Insert(i, null);
					i++;
				}
			}

			return segments;
		}
	}
}
