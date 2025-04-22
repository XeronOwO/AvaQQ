namespace AvaQQ.Core.Adapters;

/// <summary>
/// 适配的群聊信息
/// </summary>
/// <param name="Uin">群号</param>
/// <param name="Name">群名</param>
/// <param name="Remark">备注</param>
public record struct AdaptedGroupInfo(ulong Uin, string Name, string? Remark);

/// <summary>
/// 适配的用户信息
/// </summary>
/// <param name="Uin">QQ 号</param>
/// <param name="Nickname">昵称</param>
/// <param name="Remark">备注</param>
public record struct AdaptedUserInfo(ulong Uin, string Nickname, string? Remark);
