using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaQQ.Core.Databases;

/// <summary>
/// 记录的群聊记录
/// </summary>
/// <param name="Uin">群号</param>
/// <param name="Name">群名</param>
/// <param name="Remark">备注</param>
[Table("group"), Index(nameof(Name)), Index(nameof(Remark))]
public record class RecordedGroupInfo(
	[property: Column("uin"), Key, DatabaseGenerated(DatabaseGeneratedOption.None), Required] ulong Uin,
	[property: Column("name"), MaxLength(128), Required] string Name,
	[property: Column("remark"), MaxLength(128)] string? Remark
	);

/// <summary>
/// 记录的用户记录
/// </summary>
/// <param name="Uin">QQ 号</param>
/// <param name="Nickname">昵称</param>
/// <param name="Remark">备注</param>
[Table("user"), Index(nameof(Nickname)), Index(nameof(Remark))]
public record class RecordedUserInfo(
	[property: Column("uin"), Key, DatabaseGenerated(DatabaseGeneratedOption.None), Required] ulong Uin,
	[property: Column("name"), MaxLength(128), Required] string Nickname,
	[property: Column("remark"), MaxLength(128)] string? Remark
	);
