namespace AvaQQ.Core.Configurations;

internal class CacheConfiguration
{
	public TimeSpan FriendUpdateInterval { get; set; } = TimeSpan.FromMinutes(5);

	public TimeSpan UserUpdateInterval { get; set; } = TimeSpan.FromMinutes(5);

	public TimeSpan GroupUpdateInterval { get; set; } = TimeSpan.FromMinutes(5);

	public TimeSpan GroupMemberUpdateInterval { get; set; } = TimeSpan.FromMinutes(5);

	public TimeSpan FriendAvatarExpiration { get; set; } = TimeSpan.FromHours(1);

	public TimeSpan GroupAvatarExpiration { get; set; } = TimeSpan.FromHours(1);

	public uint PreSyncMessageCount { get; set; } = 10;
}
