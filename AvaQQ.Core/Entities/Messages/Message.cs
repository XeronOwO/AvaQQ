using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaQQ.Core.Entities.Messages;

/// <summary>
/// 消息<br/>
/// 类中属性由数据库自动填充，因此，如果自行创建，请手动初始化属性，否则部分属性将为 null
/// </summary>
[Table("message")]
[PrimaryKey(nameof(GroupUin), nameof(SenderUin), nameof(Sequence))]
[Index(nameof(GroupUin))]
[Index(nameof(SenderUin))]
[Index(nameof(Sequence))]
[Index(nameof(Time))]
public class Message
{
	/// <summary>
	/// 群号<br/>非群消息时为 null
	/// </summary>
	[Column]
	public ulong? GroupUin { get; set; }

	/// <summary>
	/// 发送者 QQ 号
	/// </summary>
	[Column]
	[Required]
	public ulong SenderUin { get; set; }

	/// <summary>
	/// 消息序号
	/// </summary>
	[Column]
	[Required]
	public ulong Sequence { get; set; }

	/// <summary>
	/// 时间
	/// </summary>
	[Column]
	[Required]
	public DateTimeOffset Time { get; set; }

	/// <summary>
	/// 文本段落
	/// </summary>
	public virtual List<TextSegment> Texts { get; set; } = null!;

	/// <summary>
	/// 所有段落
	/// </summary>
	public IEnumerable<ISegment?> Segments
	{
		get
		{
			var segments = new List<ISegment?>();
			segments.AddRange(Texts);

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
		set
		{
			Texts ??= [];
			Texts.Clear();

			foreach (var segment in value)
			{
				if (segment is null)
				{
					continue;
				}

				switch (segment)
				{
					case TextSegment textSegment:
						Texts.Add(textSegment);
						break;
					default:
						throw new NotImplementedException($"Unsupported segment type: {segment.GetType().FullName}");
				}
			}
		}
	}
}
