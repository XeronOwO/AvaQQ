using Avalonia.Media.Imaging;
using AvaQQ.Caches;
using AvaQQ.SDK;
using AvaQQ.SDK.Entities;

namespace AvaQQ.Contexts;

internal class CacheContext(AvatarCache avatarCache, UserCache userCache) : ICacheContext
{
	public Bitmap? GetAvatar(AvatarId id, bool forceUpdate = false)
		=> avatarCache.Get(id, forceUpdate)?.Avatar;

	public IUserInfo? GetUser(ulong uin, bool forceUpdate = false)
		=> userCache.Get(uin, forceUpdate);
}
