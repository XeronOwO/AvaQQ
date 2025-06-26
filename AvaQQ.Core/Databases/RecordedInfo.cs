using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaQQ.Core.Databases;

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
/// 记录的消息
/// </summary>
/// <param name="GroupUin">群号</param>
/// <param name="SenderUin">发送者的 QQ 号</param>
/// <param name="Sequence">消息序号</param>
/// <param name="Time">时间</param>
/// <param name="SenderDisplayName">发送者的展示名称</param>
[Table("msg"), PrimaryKey(nameof(GroupUin), nameof(SenderUin), nameof(Sequence)),
	Index(nameof(GroupUin)), Index(nameof(SenderUin)), Index(nameof(Sequence)), Index(nameof(Time))]
public record class RecordedMessage(
	[property: Column("group_uin")] ulong? GroupUin,
	[property: Column("sender_uin"), Required] ulong SenderUin,
	[property: Column("seq"), Required] ulong Sequence,
	[property: Column("time"), Required] DateTime Time,
	[property: Column("sender_disp_name"), Required] ulong SenderDisplayName
	)
{
	/// <summary>
	/// 文本段
	/// </summary>
	public ICollection<RecordedTextSegment> TextSegments { get; set; } = [];
}

/// <summary>
/// 记录的文本段
/// </summary>
/// <param name="GroupUin">群号</param>
/// <param name="SenderUin">发送者的 QQ 号</param>
/// <param name="Sequence">消息序号</param>
/// <param name="Index">段序号</param>
/// <param name="Text">文本内容</param>
[Table("txt_seg"), PrimaryKey(nameof(GroupUin), nameof(SenderUin), nameof(Sequence), nameof(Index)),
	Index(nameof(GroupUin)), Index(nameof(SenderUin)), Index(nameof(Sequence)), Index(nameof(Text))]
public record class RecordedTextSegment(
	[property: Column("group_uin")] ulong? GroupUin,
	[property: Column("sender_uin"), Required] ulong SenderUin,
	[property: Column("seq"), Required] ulong Sequence,
	[property: Column("idx"), Required] uint Index,
	[property: Column("txt"), Required] string Text
	)
{
	/// <summary>
	/// 消息
	/// </summary>
	public RecordedMessage Message { get; set; } = null!;
}
