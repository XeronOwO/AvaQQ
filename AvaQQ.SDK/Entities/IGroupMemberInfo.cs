namespace AvaQQ.SDK.Entities;

public interface IGroupMemberInfo : IUpdateTime
{
	ulong MemberUin { get; set; }

	ulong GroupUin { get; set; }

	bool IsIn { get; set; }

	string? MemberGroupNickname { get; set; }

	GroupMemberPermission Permission { get; set; }
}
