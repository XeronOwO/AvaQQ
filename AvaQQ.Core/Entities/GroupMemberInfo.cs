using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaQQ.Core.Entities;

/// <summary>
/// 群员信息
/// </summary>
[Table("group_member")]
[PrimaryKey(nameof(MemberUin), nameof(GroupUin))]
[Index(nameof(MemberUin))]
[Index(nameof(GroupUin))]
[Index(nameof(IsIn))]
public class GroupMemberInfo : IUpdateTime
{
	/// <summary>
	/// 群员 QQ 号
	/// </summary>
	[Column]
	[Required]
	public ulong MemberUin { get; set; }

	/// <summary>
	/// 群号
	/// </summary>
	[Column]
	[Required]
	public ulong GroupUin { get; set; }

	/// <summary>
	/// 是否在群内
	/// </summary>
	[Column]
	[Required]
	public bool IsIn { get; set; }

	/// <summary>
	/// 群内昵称
	/// </summary>
	[Column]
	[MaxLength(128)]
	public string? MemberGroupNickname { get; set; }

	/// <summary>
	/// 权限
	/// </summary>
	[Column]
	[Required]
	public GroupMemberPermission Permission { get; set; }

	/// <summary>
	/// 更新时间
	/// </summary>
	[Column]
	[Required]
	public DateTimeOffset UpdateTime { get; set; }
}
