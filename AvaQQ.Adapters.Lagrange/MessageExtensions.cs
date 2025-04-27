using AvaQQ.Core.Messages;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;
using Microsoft.Extensions.Logging;

namespace AvaQQ.Adapters.Lagrange;

internal static class MessageExtensions
{
	public static AtSegment ToSegment(this MentionEntity entity)
		=> new(entity.Uin, entity.Name ?? string.Empty);
	
	public static TextSegment ToSegment(this TextEntity entity)
		=> new(entity.Text);

	public static ISegment? ToSegment(this IMessageEntity entity, ILogger<ISegment> logger)
	{
		switch (entity)
		{
			case MentionEntity mention:
				return mention.ToSegment();
			case TextEntity text:
				return text.ToSegment();
			default:
				logger.LogWarning("Unsupported message entity type: {Type}", entity.GetType());
				return null;
		}
	}
	
	public static Message ToMessage(this MessageChain chain, ILogger<ISegment> logger)
	{
		Message message;
		if (chain.GroupUin != null)
		{
			message = new(
				chain.GroupUin,
				chain.TargetUin,
				string.IsNullOrEmpty(chain.GroupMemberInfo!.MemberCard)
					? chain.GroupMemberInfo!.MemberName
					: chain.GroupMemberInfo!.MemberCard,
				chain.Time,
				chain.Sequence
				);
		}
		else
		{
			message = new(
				chain.GroupUin,
				chain.FriendUin,
				string.IsNullOrEmpty(chain.FriendInfo!.Remarks)
					? chain.FriendInfo!.Nickname
					: chain.FriendInfo!.Remarks,
				chain.Time,
				chain.Sequence
				);
		}

		foreach (var entity in chain)
		{
			if (entity.ToSegment(logger) is { } segment)
			{
				message.Add(segment);
			}
		}

		return message;
	}
}
