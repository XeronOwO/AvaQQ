namespace AvaQQ.Core.Entities;

/// <summary>
/// 群员权限
/// </summary>
public enum GroupMemberPermission : byte
{
	/// <summary>
	/// 群主
	/// </summary>
	Owner,
	/// <summary>
	/// 管理员
	/// </summary>
	Administrator,
	/// <summary>
	/// 成员
	/// </summary>
	Member,
}
