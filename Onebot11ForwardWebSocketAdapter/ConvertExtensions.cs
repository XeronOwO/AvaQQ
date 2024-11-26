using Microsoft.Extensions.Logging;
using AGroupMessageType = AvaQQ.SDK.Databases.GroupMessageType;
using AGroupRoleType = AvaQQ.SDK.Adapters.GroupRoleType;
using MGroupMessageEventType = Makabaka.Events.GroupMessageEventType;
using MGroupRoleType = Makabaka.Models.GroupRoleType;

namespace Onebot11ForwardWebSocketAdapter;

internal static class ConvertExtensions
{
	public static AGroupMessageType ToAvaQQ(this MGroupMessageEventType type)
	{
		return type switch
		{
			MGroupMessageEventType.Normal => AGroupMessageType.Normal,
			MGroupMessageEventType.Anonymous => AGroupMessageType.Anonymous,
			MGroupMessageEventType.Notice => AGroupMessageType.Notice,
			_ => throw new ArgumentOutOfRangeException(nameof(type)),
		};
	}

	public static AGroupRoleType ToAvaQQ(this MGroupRoleType type)
	{
		return type switch
		{
			MGroupRoleType.Owner => AGroupRoleType.Owner,
			MGroupRoleType.Admin => AGroupRoleType.Admin,
			MGroupRoleType.Member => AGroupRoleType.Member,
			_ => throw new ArgumentOutOfRangeException(nameof(type)),
		};
	}

	public static AvaQQ.SDK.Messages.Message ToAvaQQ(this Makabaka.Messages.Message message, ILogger logger)
	{
		var result = new AvaQQ.SDK.Messages.Message();
		foreach (var segment in message)
		{
			switch (segment)
			{
				case Makabaka.Messages.TextSegment text:
					result.Add(new AvaQQ.SDK.Messages.TextSegment()
					{
						Text = text.Data.Text,
					});
					break;
				default:
					logger.LogWarning("Unsupported segment type: {Type}", segment.GetType());
					break;
			}
		}
		return result;
	}
}
