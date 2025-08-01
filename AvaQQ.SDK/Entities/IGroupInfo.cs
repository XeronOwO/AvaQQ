namespace AvaQQ.SDK.Entities;

public interface IGroupInfo
{
	ulong Uin { get; set; }

	string Name { get; set; }

	bool IsMeIn { get; set; }

	string? Remark { get; set; }

	DateTimeOffset UpdateTime { get; set; }
}
