using AvaQQ.SDK.Messages;
using System;

namespace AvaQQ.SDK.Databases;

/// <summary>
/// 群消息
/// </summary>
public class GroupMessageEntry
{
	/// <summary>
	/// 主键
	/// </summary>
	public long Id { get; set; }

	/// <summary>
	/// 类型
	/// </summary>
	public GroupMessageType Type { get; set; }

	/// <summary>
	/// 消息 ID
	/// </summary>
	public ulong MessageId { get; set; }

	/// <summary>
	/// 时间
	/// </summary>
	public DateTimeOffset Time { get; set; }

	/// <summary>
	/// 发送者 QQ 号<br/>
	/// 如果为匿名消息，则为匿名用户 ID
	/// </summary>
	public ulong SenderUin { get; set; }

	/// <summary>
	/// 匿名标识，在调用禁言 API 时需要传入
	/// </summary>
	public string AnonymousFlag { get; set; } = string.Empty;

	/// <summary>
	/// 消息内容
	/// </summary>
	public Message Message { get; set; } = [];
}
