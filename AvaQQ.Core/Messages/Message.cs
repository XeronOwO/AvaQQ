using System.Text;

namespace AvaQQ.Core.Messages;

/// <summary>
/// 消息
/// </summary>
/// <param name="groupUin">群号<br/>如果是私聊消息，则为 null</param>
/// <param name="senderUin">发送者的 QQ 号</param>
/// <param name="senderDisplayName">发送者的展示名称</param>
/// <param name="time">发送时间</param>
/// <param name="sequence">消息序号</param>
public class Message(ulong? groupUin, ulong senderUin, string senderDisplayName, DateTime time, ulong sequence) : List<ISegment>
{
	/// <summary>
	/// 群号<br/>
	/// 如果是私聊消息，则为 null
	/// </summary>
	public ulong? GroupUin { get; } = groupUin;

	/// <summary>
	/// 发送者的 QQ 号
	/// </summary>
	public ulong SenderUin { get; } = senderUin;

	/// <summary>
	/// 发送者的展示名称
	/// </summary>
	public string SenderDisplayName { get; } = senderDisplayName;

	/// <summary>
	/// 发送时间
	/// </summary>
	public DateTime Time { get; } = time;

	/// <summary>
	/// 消息序号
	/// </summary>
	public ulong Sequence { get; } = sequence;

	/// <inheritdoc/>
	public string Preview
	{
		get
		{
			var sb = new StringBuilder();
			sb.Append(SenderDisplayName);
			sb.Append(": ");
			foreach (var segment in this)
			{
				sb.Append(segment.Preview);
			}
			return sb.ToString();
		}
	}
}
