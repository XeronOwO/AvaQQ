using AvaQQ.Core.Events;

namespace AvaQQ.Core.Caches;

/// <summary>
/// 头像缓存 ID
/// </summary>
public struct AvatarCacheId : IEventId
{
	/// <summary>
	/// QQ 号或群号
	/// </summary>
	public required ulong Uin { get; set; }

	/// <summary>
	/// 头像尺寸
	/// </summary>
	public required int Size { get; set; }

	/// <inheritdoc/>
	public readonly bool Equals(IEventId? other)
	{
		if (other is not AvatarCacheId o)
		{
			return false;
		}

		return Uin == o.Uin && Size == o.Size;
	}
}
