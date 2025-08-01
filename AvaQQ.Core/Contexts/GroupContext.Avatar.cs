using Avalonia.Media.Imaging;
using AvaQQ.Core.Entities;
using AvaQQ.Core.Events;
using Microsoft.Extensions.Logging;

namespace AvaQQ.Core.Contexts;

internal partial class GroupContext
{
	private void InitializeAvatar()
	{
		_events.OnGroupAvatarFetched.Subscribe(OnAvatarFetched);
	}

	private void FinalizeAvatar()
	{
		_events.OnGroupAvatarFetched.Unsubscribe(OnAvatarFetched);
	}

	private readonly IAvatarCache _avatarCache;

	public Bitmap? GetAvatar(ulong uin, uint size = 0, bool forceUpdate = false)
	{
		var cache = _avatarCache.Get(uin, size);

		if (forceUpdate || cache.RequiresUpdate)
		{
			_events.OnUserAvatarFetched.Invoke(
				new AvatarId(uin, size),
				() => FetchAvatarFromUrlAsync(uin, size)
				);
		}

		return cache.Avatar;
	}

	private async Task<byte[]?> FetchAvatarFromUrlAsync(ulong uin, uint size)
	{
		var url = $"https://p.qlogo.cn/gh/{uin}/{uin}/{size}";
		try
		{
			_logger.LogDebug("Downloading group {Uin}'s avatar of size {Size}.", uin, size);

			var response = await Shared.HttpClient.GetAsync(url);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsByteArrayAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to fetch group {Uin}'s avatar of size {Size} from url {Url}.", uin, size, url);
			return null;
		}
	}

	private void OnAvatarFetched(object? sender, BusEventArgs<AvatarId, byte[]?> e)
	{
		if (!_avatarCache.TryAddOrUpdate(e.Id.Uin, e.Id.Size, e.Result, out var info))
		{
			return;
		}

		_events.OnAvatarChanged.Invoke(e.Id, info.Value);
	}
}
