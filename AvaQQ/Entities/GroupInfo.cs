using AvaQQ.SDK.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaQQ.Entities;

[Table("group")]
[PrimaryKey(nameof(Uin))]
[Index(nameof(Name))]
[Index(nameof(IsMeIn))]
[Index(nameof(Remark))]
public class GroupInfo : IGroupInfo
{
	[Column]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	[Required]
	public ulong Uin { get; set; }

	[Column]
	[MaxLength(128)]
	[Required]
	public string Name { get; set; } = null!;

	[Column]
	[Required]
	public bool IsMeIn { get; set; }

	[Column]
	[MaxLength(128)]
	public string? Remark { get; set; }

	[Column]
	[Required]
	public DateTimeOffset UpdateTime { get; set; }
}
