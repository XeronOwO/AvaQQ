namespace Onebot11ForwardWebSocketAdapter;

internal static class ConvertExtensions
{
	public static AvaQQ.SDK.Databases.GroupMessageType ToAvaQQ(this Makabaka.Events.GroupMessageEventType type)
	{
		return type switch
		{
			Makabaka.Events.GroupMessageEventType.Normal => AvaQQ.SDK.Databases.GroupMessageType.Normal,
			Makabaka.Events.GroupMessageEventType.Anonymous => AvaQQ.SDK.Databases.GroupMessageType.Anonymous,
			Makabaka.Events.GroupMessageEventType.Notice => AvaQQ.SDK.Databases.GroupMessageType.Notice,
			_ => throw new ArgumentOutOfRangeException(nameof(type)),
		};
	}

	public static AvaQQ.SDK.Adapters.GroupRoleType ToAvaQQ(this Makabaka.Models.GroupRoleType type)
	{
		return type switch
		{
			Makabaka.Models.GroupRoleType.Owner => AvaQQ.SDK.Adapters.GroupRoleType.Owner,
			Makabaka.Models.GroupRoleType.Admin => AvaQQ.SDK.Adapters.GroupRoleType.Admin,
			Makabaka.Models.GroupRoleType.Member => AvaQQ.SDK.Adapters.GroupRoleType.Member,
			_ => throw new ArgumentOutOfRangeException(nameof(type)),
		};
	}
}
