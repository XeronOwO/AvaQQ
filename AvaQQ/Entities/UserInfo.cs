using AvaQQ.SDK.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaQQ.Entities;

[Table("user")]
[PrimaryKey(nameof(Uin))]
[Index(nameof(Nickname))]
[Index(nameof(IsFriend))]
[Index(nameof(Remark))]
public class UserInfo : IUserInfo
{
	[Column]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	[Required]
	public ulong Uin { get; set; }

	[Column]
	[MaxLength(128)]
	[Required]
	public string Nickname { get; set; } = null!;

	[Column]
	[Required]
	public bool IsFriend { get; set; }

	[Column]
	[MaxLength(128)]
	public string? Remark { get; set; }

	[Column]
	[Required]
	public DateTimeOffset UpdateTime { get; set; }

	public static UserInfo FromAdapted(AdaptedUserInfo adapted) => new()
	{
		Uin = adapted.Uin,
		Nickname = adapted.Nickname,
		IsFriend = false,
		Remark = adapted.Remark,
		UpdateTime = DateTimeOffset.Now,
	};
}
