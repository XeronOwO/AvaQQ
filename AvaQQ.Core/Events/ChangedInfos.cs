using Avalonia.Media.Imaging;

namespace AvaQQ.Core.Events;

/// <summary>
/// 头像改变信息
/// </summary>
/// <param name="Old">旧头像</param>
/// <param name="New">新头像</param>
public record struct AvatarChangedInfo(Bitmap? Old, Bitmap New);

/// <summary>
/// 用户昵称改变信息
/// </summary>
/// <param name="Old">旧昵称</param>
/// <param name="New">新昵称</param>
/// <param name="Cache">用户缓存</param>
public record struct UserNicknameChangedInfo(string Old, string New, CachedUserInfo Cache);

/// <summary>
/// 群名称改变信息
/// </summary>
/// <param name="Old">旧名称</param>
/// <param name="New">新名称</param>
/// <param name="Cache">群缓存</param>
public record struct GroupNameChangedInfo(string Old, string New, CachedGroupInfo Cache);

/// <summary>
/// 用户备注改变信息
/// </summary>
/// <param name="Old">旧备注</param>
/// <param name="New">新备注</param>
/// <param name="Cache">用户缓存</param>
public record struct UserRemarkChangedInfo(string? Old, string? New, CachedUserInfo Cache);

/// <summary>
/// 群备注改变信息
/// </summary>
/// <param name="Old">旧备注</param>
/// <param name="New">新备注</param>
/// <param name="Cache">用户缓存</param>
public record struct GroupRemarkChangedInfo(string? Old, string? New, CachedGroupInfo Cache);
