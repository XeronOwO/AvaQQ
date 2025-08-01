using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaQQ.Core.Entities;

/// <summary>
/// 用户信息
/// </summary>
[Table("user")]
[PrimaryKey(nameof(Uin))]
[Index(nameof(Nickname))]
[Index(nameof(IsFriend))]
[Index(nameof(Remark))]
public class UserInfo : IUpdateTime
{
	/// <summary>
	/// QQ 号
	/// </summary>
	[Column]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	[Required]
	public ulong Uin { get; set; }

	/// <summary>
	/// 昵称
	/// </summary>
	[Column]
	[MaxLength(128)]
	[Required]
	public string Nickname { get; set; } = null!;

	/// <summary>
	/// 是否是好友
	/// </summary>
	[Column]
	[Required]
	public bool IsFriend { get; set; }

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
