namespace AvaQQ.SDK.Entities;

public interface IUserInfo : IUpdateTime
{
	ulong Uin { get; set; }

	string Nickname { get; set; }

	bool IsFriend { get; set; }

	string? Remark { get; set; }

	IUserInfo Update(AdaptedUserInfo info, bool? isFriend = null)
	{
		Uin = info.Uin;
		Nickname = info.Nickname;
		Remark = info.Remark;
		UpdateTime = DateTimeOffset.Now;
		if (isFriend.HasValue)
		{
			IsFriend = isFriend.Value;
		}
		return this;
	}
}
