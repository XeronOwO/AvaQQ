﻿using Microsoft.Extensions.Logging;
using AGroupMessageType = AvaQQ.SDK.Databases.GroupMessageType;
using AGroupRoleType = AvaQQ.SDK.Adapters.GroupRoleType;
using ASexType = AvaQQ.SDK.Adapters.SexType;
using MGroupMessageEventType = Makabaka.Events.GroupMessageEventType;
using MGroupRoleType = Makabaka.Models.GroupRoleType;
using MSexType = Makabaka.Models.SexType;

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

	public static ASexType ToAvaQQ(this MSexType type)
	{
		return type switch
		{
			MSexType.Male => ASexType.Male,
			MSexType.Female => ASexType.Female,
			MSexType.Unknown => ASexType.Unknown,
			_ => throw new ArgumentOutOfRangeException(nameof(type)),
		};
	}

	public static AvaQQ.SDK.Messages.Segment? ToAvaQQ(this Makabaka.Messages.Segment segment, ILogger logger)
	{
		switch (segment)
		{
			case Makabaka.Messages.AtSegment at:
				return new AvaQQ.SDK.Messages.AtSegment()
				{
					Uin = at.Data.QQ == "all" ? 0 : ulong.Parse(at.Data.QQ),
				};
			case Makabaka.Messages.FaceSegment face:
				return new AvaQQ.SDK.Messages.FaceSegment()
				{
					Id = ulong.Parse(face.Data.Id),
					IsLarge = face.Data.IsLarge,
				};
			case Makabaka.Messages.ForwardSegment forward:
				return new AvaQQ.SDK.Messages.ForwardSegment()
				{
					ResId = forward.Data.Id,
				};
			case Makabaka.Messages.ImageSegment image:
				return new AvaQQ.SDK.Messages.ImageSegment()
				{
					Filename = image.Data.Filename!,
					Url = image.Data.Url!,
					SubType = image.Data.SubType,
				};
			case Makabaka.Messages.NodeSegment node:
				return new AvaQQ.SDK.Messages.NodeSegment()
				{
					Uin = ulong.Parse(node.Data.Id!),
					DisplayName = node.Data.Nickname!,
					Content = node.Data.Content!.ToAvaQQ(logger),
				};
			case Makabaka.Messages.ReplySegment reply:
				return new AvaQQ.SDK.Messages.ReplySegment()
				{
					MessageId = (ulong)long.Parse(reply.Data.Id),
				};
			case Makabaka.Messages.TextSegment text:
				return new AvaQQ.SDK.Messages.TextSegment()
				{
					Text = text.Data.Text,
				};
			default:
				logger.LogWarning("Unsupported segment type: {Type}", segment.GetType());
				return null;
		}
	}

	public static AvaQQ.SDK.Messages.Message ToAvaQQ(this Makabaka.Messages.Message message, ILogger logger)
	{
		var result = new AvaQQ.SDK.Messages.Message();
		foreach (var segment in message)
		{
			var segment1 = segment.ToAvaQQ(logger);
			if (segment1 is not null)
			{
				result.Add(segment1);
			}
		}
		return result;
	}
}
