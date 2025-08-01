using Avalonia.Media.Imaging;
using AvaQQ.SDK.Entities;

namespace AvaQQ.SDK;

public interface ICacheContext
{
	Bitmap? GetAvatar(AvatarId id, bool forceUpdate = false);

	IUserInfo? GetUser(ulong uin, bool forceUpdate = false);
}
