using System;

namespace AvaQQ.Configurations;

internal class CacheConfiguration
{
	public TimeSpan FriendUpdateInterval { get; set; } = TimeSpan.FromMinutes(5);

	public TimeSpan UserUpdateInterval { get; set; } = TimeSpan.FromMinutes(5);

	public TimeSpan GroupUpdateInterval { get; set; } = TimeSpan.FromMinutes(5);

	public TimeSpan GroupMemberUpdateInterval { get; set; } = TimeSpan.FromMinutes(5);
}
