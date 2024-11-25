using System;

namespace AvaQQ.Configurations;

internal class FriendConfiguration
{
	public TimeSpan FriendListUpdateInterval { get; set; } = TimeSpan.FromMinutes(5);
}
