using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaQQ.Core.Entities;

/// <summary>
/// 群信息
/// </summary>
[Table("group")]
[PrimaryKey(nameof(Uin))]
[Index(nameof(Name))]
[Index(nameof(IsMeIn))]
[Index(nameof(Remark))]
public class GroupInfo : IUpdateTime
{
	/// <summary>
	/// 群号
	/// </summary>
	[Column]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	[Required]
	public ulong Uin { get; set; }

	/// <summary>
	/// 群名
	/// </summary>
	[Column]
	[MaxLength(128)]
	[Required]
	public string Name { get; set; } = null!;

	/// <summary>
	/// 是否我在群内
	/// </summary>
	[Column]
	[Required]
	public bool IsMeIn { get; set; }

	/// <summary>
	/// 备注
	/// </summary>
	[Column]
	[MaxLength(128)]
	public string? Remark { get; set; }

	/// <summary>
	/// 更新时间
	/// </summary>
	[Column]
	[Required]
	public DateTimeOffset UpdateTime { get; set; }
}
