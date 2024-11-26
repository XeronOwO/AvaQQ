using AvaQQ.SDK.Databases;
using AvaQQ.SDK.Messages;
using System;

namespace AvaQQ.SDK.Adapters;

/// <summary>
/// 群消息事件参数
/// </summary>
public class GroupMessageEventArgs
{
	/// <summary>
	/// 类型
	/// </summary>
	public GroupMessageType Type { get; set; }

	/// <summary>
	/// 消息 ID
	/// </summary>
	public long MessageId { get; set; }

	/// <summary>
	/// 时间
	/// </summary>
	public DateTime Time { get; set; }

	/// <summary>
	/// 群号
	/// </summary>
	public long GroupUin { get; set; }

	/// <summary>
	/// 是否为匿名消息
	/// </summary>
	public bool IsAnonymous { get; set; }

	/// <summary>
	/// 发送者 QQ 号<br/>
	/// 如果为匿名消息，则为匿名用户 ID
	/// </summary>
	public long SenderUin { get; set; }

	/// <summary>
	/// 匿名标识，在调用禁言 API 时需要传入
	/// </summary>
	public string AnonymousFlag { get; set; } = string.Empty;

	/// <summary>
	/// 消息内容
	/// </summary>
	public Message Message { get; set; } = [];

	/// <summary>
	/// 发送者的昵称
	/// </summary>
	public string SenderNickname { get; set; } = string.Empty;

	/// <summary>
	/// 发送者的群内昵称
	/// </summary>
	public string SenderGroupNickname { get; set; } = string.Empty;

	/// <summary>
	/// 自己设置的发送者的备注
	/// </summary>
	public string SenderRemark { get; set; } = string.Empty;

	/// <summary>
	/// 发送者的群等级
	/// </summary>
	public int SenderLevel { get; set; }

	/// <summary>
	/// 发送者的群角色
	/// </summary>
	public GroupRoleType SenderRole { get; set; }

	/// <summary>
	/// 发送者的专属群头衔
	/// </summary>
	public string SpecificTitle { get; set; } = string.Empty;

	/// <summary>
	/// 隐式转换为 <see cref="GroupMessageEntry"/>
	/// </summary>
	public static implicit operator GroupMessageEntry(GroupMessageEventArgs e)
	{
		return new GroupMessageEntry()
		{
			Type = e.Type,
			MessageId = e.MessageId,
			Time = e.Time,
			GroupUin = e.GroupUin,
			IsAnonymous = e.IsAnonymous,
			SenderUin = e.SenderUin,
			AnonymousFlag = e.AnonymousFlag,
			Message = e.Message,
		};
	}
}
