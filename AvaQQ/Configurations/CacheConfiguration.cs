namespace AvaQQ.Configurations;

internal class CacheConfiguration
{
	public TimeSpan AvatarExpiration { get; set; } = TimeSpan.FromHours(1);

	public TimeSpan UserExpiration { get; set; } = TimeSpan.FromHours(1);
}
