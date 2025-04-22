namespace AvaQQ.Core.Databases;

/// <summary>
/// 记录的群聊记录
/// </summary>
/// <param name="Uin">群号</param>
/// <param name="Name">群名</param>
/// <param name="Remark">备注</param>
public record struct RecordedGroupInfo(ulong Uin, string Name, string? Remark);

/// <summary>
/// 记录的用户记录
/// </summary>
/// <param name="Uin">QQ 号</param>
/// <param name="Nickname">昵称</param>
/// <param name="Remark">备注</param>
public record struct RecordedUserInfo(ulong Uin, string Nickname, string? Remark);
