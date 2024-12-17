using Avalonia.Media.Imaging;
using AvaQQ.Core.Utils;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.CacheConfiguration>;

namespace AvaQQ.Core.Caches;

internal class AvatarCache : IAvatarCache
{
	private readonly IServiceProvider _serviceProvider;

	private readonly string _baseDirectory;

	private readonly string _usersDirectory;

	private readonly ILogger<AvatarCache> _logger;

	public AvatarCache(IServiceProvider serviceProvider)
	{
		CirculationInjectionDetector<AvatarCache>.Enter();

		_serviceProvider = serviceProvider;
		_baseDirectory = Path.Combine(Constants.RootDirectory, "cache", "avatars");
		_usersDirectory = Path.Combine(_baseDirectory, "users");
		_logger = _serviceProvider.GetRequiredService<ILogger<AvatarCache>>();

		CirculationInjectionDetector<AvatarCache>.Leave();
	}

	#region 用户

	public Task<Bitmap?> GetUserAvatarAsync(ulong uin, int size = 0, bool noCache = false)
	{
		if (!Directory.Exists(_usersDirectory))
		{
			Directory.CreateDirectory(_usersDirectory);
		}

		if (noCache)
		{
			return GetUserAvatarNoCacheAsync(uin, size);
		}
		else
		{
			return GetUserAvatarFromCacheAsync(uin, size);
		}
	}

	private async Task<Bitmap?> GetUserAvatarNoCacheAsync(ulong uin, int size)
	{
		try
		{
			var response = await Shared.HttpClient.GetAsync($"https://q1.qlogo.cn/g?b=qq&nk={uin}&s={size}");
			response.EnsureSuccessStatusCode();

			var bytes = await response.Content.ReadAsByteArrayAsync();
			var type = bytes.GetMediaType();
			if (type == MediaType.Unknown)
			{
				return null;
			}

			var directory = Path.Combine(_usersDirectory, size.ToString());
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var path = Path.Combine(directory, $"{uin}{type.GetFileExtension()}");
			await File.WriteAllBytesAsync(path, bytes);
			using var stream = new MemoryStream(bytes);
			return new Bitmap(stream);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get user {Uin}'s avatar of size {Size}.", uin, size);
			return null;
		}
	}

	private async Task<Bitmap?> GetUserAvatarFromCacheAsync(ulong uin, int size)
	{
		try
		{
			var directory = Path.Combine(_usersDirectory, size.ToString());
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var paths = Directory.GetFiles(directory, $"{uin}.*");
			var path = paths.FirstOrDefault();
			if (string.IsNullOrEmpty(path)
				|| !File.Exists(path))
			{
				return await GetUserAvatarNoCacheAsync(uin, size);
			}

			return new Bitmap(path);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get user {Uin}'s avatar of size {Size}.", uin, size);
			return null;
		}
	}

	#endregion

	#region 群聊

	private async Task<Bitmap?> GetGroupAvatarFromWebsiteAsync(ulong uin, int size)
	{
		try
		{
			var response = await Shared.HttpClient.GetAsync($"https://p.qlogo.cn/gh/{uin}/{uin}/{size}");
			response.EnsureSuccessStatusCode();
			using var stream = await response.Content.ReadAsStreamAsync();
			return new Bitmap(stream);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get group {Uin}'s avatar of size {Size}.", uin, size);
			return null;
		}
	}

	private class GroupAvatarCache
	{
		public required DateTime LastUpdateTime { get; set; }

		public required Task<Bitmap?> Avatar { get; set; }
	}

	private readonly Dictionary<ulong, Dictionary<int, GroupAvatarCache>> _groups = [];

	public Task<Bitmap?> GetGroupAvatarAsync(ulong uin, int size = 0, bool noCache = false)
	{
		if (!_groups.TryGetValue(uin, out var caches))
		{
			caches = _groups[uin] = [];
		}

		var now = DateTime.Now;
		if (!caches.TryGetValue(size, out var cache))
		{
			cache = caches[size] = new GroupAvatarCache()
			{
				LastUpdateTime = now,
				Avatar = GetGroupAvatarFromWebsiteAsync(uin, size)
			};
		}

		if (now > cache.LastUpdateTime + Config.Instance.GroupAvatarExpiration)
		{
			cache.LastUpdateTime = now;
			cache.Avatar = GetGroupAvatarFromWebsiteAsync(uin, size);
		}

		return cache.Avatar;
	}

	public void ReleaseGroupAvatars()
	{
		_groups.Clear();
	}

	#endregion
}
