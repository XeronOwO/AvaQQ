namespace AvaQQ.Core.Events;

/// <summary>
/// 一些通用的事件 ID
/// </summary>
public readonly struct CommonEventId : IEventId
{
	/// <summary>
	/// 事件 ID
	/// </summary>
	public required EventId Id { get; init; }

	/// <inheritdoc/>
	public readonly bool Equals(IEventId? other)
	{
		if (other is not CommonEventId o)
		{
			return false;
		}

		return Id == o.Id;
	}

	/// <inheritdoc/>
	public override readonly int GetHashCode()
	{
		return Id.GetHashCode();
	}

	/// <summary>
	/// 事件 ID
	/// </summary>
	public enum EventId
	{
		/// <inheritdoc cref="CommonEventId.GetAllRecordedUsers"/>
		GetAllRecordedUsers,
		/// <inheritdoc cref="CommonEventId.GetAllFriends"/>
		GetAllFriends,
		/// <inheritdoc cref="CommonEventId.CachedGetAllFriends"/>
		CachedGetAllFriends,
		/// <inheritdoc cref="CommonEventId.GetAllRecordedGroups"/>
		GetAllRecordedGroups,
		/// <inheritdoc cref="CommonEventId.GetAllJoinedGroups"/>
		GetAllJoinedGroups,
		/// <inheritdoc cref="CommonEventId.CachedGetAllJoinedGroups"/>
		CachedGetAllJoinedGroups,
		/// <inheritdoc cref="CommonEventId.GroupMessage"/>
		GroupMessage,
	}

	/// <summary>
	/// 当从数据库中加载所有用户信息后触发
	/// </summary>
	public static CommonEventId GetAllRecordedUsers { get; } = new()
	{
		Id = EventId.GetAllRecordedUsers,
	};

	/// <summary>
	/// 当从服务器获取所有好友后触发
	/// </summary>
	public static CommonEventId GetAllFriends { get; } = new()
	{
		Id = EventId.GetAllFriends,
	};

	/// <summary>
	/// 当从服务器获取所有好友、并更新缓存后触发<br/>
	/// 在 <see cref="GetAllFriends"/> 期间触发
	/// </summary>
	public static CommonEventId CachedGetAllFriends { get; } = new()
	{
		Id = EventId.CachedGetAllFriends,
	};

	/// <summary>
	/// 当从数据库中加载所有群信息后触发
	/// </summary>
	public static CommonEventId GetAllRecordedGroups { get; } = new()
	{
		Id = EventId.GetAllRecordedGroups,
	};

	/// <summary>
	/// 当从服务器获取所有加入的群后触发
	/// </summary>
	public static CommonEventId GetAllJoinedGroups { get; } = new()
	{
		Id = EventId.GetAllJoinedGroups,
	};

	/// <summary>
	/// 当从服务器获取所有加入的群、并更新缓存后触发<br/>
	/// 在 <see cref="GetAllJoinedGroups"/> 期间触发
	/// </summary>
	public static CommonEventId CachedGetAllJoinedGroups { get; } = new()
	{
		Id = EventId.CachedGetAllJoinedGroups,
	};

	/// <summary>
	/// 当接收到群消息时触发
	/// </summary>
	public static CommonEventId GroupMessage { get; } = new()
	{
		Id = EventId.GroupMessage,
	};
}
