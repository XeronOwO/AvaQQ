using System;

namespace AvaQQ.SDK.Adapters;

/// <summary>
/// 群成员信息
/// </summary>
public class GroupMemberInfo
{
	/// <summary>
	/// QQ 号
	/// </summary>
	public ulong Uin { get; set; }

	/// <summary>
	/// 群号
	/// </summary>
	public ulong GroupUin { get; set; }

	/// <summary>
	/// 群名片
	/// </summary>
	public string Card { get; set; } = string.Empty;

	/// <summary>
	/// 加群时间
	/// </summary>
	public DateTimeOffset JoinTime { get; set; }

	/// <summary>
	/// 最后发言时间
	/// </summary>
	public DateTimeOffset LastSpeakTime { get; set; }

	/// <summary>
	/// 等级
	/// </summary>
	public uint Level { get; set; }

	/// <summary>
	/// 角色
	/// </summary>
	public GroupRoleType Role { get; set; } = GroupRoleType.Member;

	/// <summary>
	/// 头衔
	/// </summary>
	public string Title { get; set; } = string.Empty;
}
