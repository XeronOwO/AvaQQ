using AvaQQ.SDK.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaQQ.Entities;

[Table("group_member")]
[PrimaryKey(nameof(MemberUin), nameof(GroupUin))]
[Index(nameof(MemberUin))]
[Index(nameof(GroupUin))]
[Index(nameof(IsIn))]
public class GroupMemberInfo : IGroupMemberInfo
{
	[Column]
	[Required]
	public ulong MemberUin { get; set; }

	[Column]
	[Required]
	public ulong GroupUin { get; set; }

	[Column]
	[Required]
	public bool IsIn { get; set; }

	[Column]
	[MaxLength(128)]
	public string? MemberGroupNickname { get; set; }

	[Column]
	[Required]
	public GroupMemberPermission Permission { get; set; }

	[Column]
	[Required]
	public DateTimeOffset UpdateTime { get; set; }
}
