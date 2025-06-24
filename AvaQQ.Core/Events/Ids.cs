namespace AvaQQ.Core.Events;

/// <summary>
/// 头像缓存 ID
/// </summary>
/// <param name="Uin">QQ 号或群号</param>
/// <param name="Size">头像尺寸</param>
public record struct AvatarId(ulong Uin, int Size) : IEquatable<AvatarId>;

/// <summary>
/// Uin ID
/// </summary>
/// <param name="Uin">QQ 号或群号</param>
public record struct UinId(ulong Uin) : IEquatable<UinId>;
