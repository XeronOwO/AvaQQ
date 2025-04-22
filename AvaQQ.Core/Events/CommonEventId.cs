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
		/// <inheritdoc cref="EventStation.GetAllFriends"/>
		GetAllFriends,
		/// <inheritdoc cref="EventStation.CachedGetAllFriends"/>
		CachedGetAllFriends,
		/// <inheritdoc cref="EventStation.GetAllJoinedGroups"/>
		GetAllJoinedGroups,
		/// <inheritdoc cref="EventStation.CachedGetAllJoinedGroups"/>
		CachedGetAllJoinedGroups,
	}

	/// <inheritdoc cref="EventStation.GetAllFriends"/>
	public static CommonEventId GetAllFriends { get; } = new()
	{
		Id = EventId.GetAllFriends,
	};

	/// <inheritdoc cref="EventStation.CachedGetAllFriends"/>
	public static CommonEventId CachedGetAllFriends { get; } = new()
	{
		Id = EventId.CachedGetAllFriends,
	};

	/// <inheritdoc cref="EventStation.GetAllJoinedGroups"/>
	public static CommonEventId GetAllJoinedGroups { get; } = new()
	{
		Id = EventId.GetAllJoinedGroups,
	};

	/// <inheritdoc cref="EventStation.CachedGetAllJoinedGroups"/>
	public static CommonEventId CachedGetAllJoinedGroups { get; } = new()
	{
		Id = EventId.CachedGetAllJoinedGroups,
	};
}
