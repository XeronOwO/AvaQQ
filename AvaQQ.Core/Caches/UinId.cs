using AvaQQ.Core.Events;

namespace AvaQQ.Core.Caches;

/// <summary>
/// Uin ID
/// </summary>
public struct UinId : IEventId
{
	/// <summary>
	/// Uin
	/// </summary>
	public required ulong Uin { get; set; }

	/// <inheritdoc/>
	public readonly bool Equals(IEventId? other)
	{
		if (other is not UinId o)
		{
			return false;
		}

		return Uin == o.Uin;
	}
}
