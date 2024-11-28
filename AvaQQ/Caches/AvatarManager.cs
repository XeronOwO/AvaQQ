using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AvaQQ.Caches;

internal class AvatarManager : IAvatarManager
{
	private readonly IServiceProvider _serviceProvider;

	private readonly string _baseDirectory;

	private readonly string _usersDirectory;

	private readonly string _groupsDirectory;

	private readonly ILogger<AvatarManager> _logger;

	public AvatarManager(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_baseDirectory = Path.Combine(Constants.RootDirectory, "cache", "avatars");
		_usersDirectory = Path.Combine(_baseDirectory, "users");
		_groupsDirectory = Path.Combine(_baseDirectory, "groups");
		_logger = _serviceProvider.GetRequiredService<ILogger<AvatarManager>>();
	}

	#region 用户

	public Task<IImage?> GetUserAvatarAsync(ulong uin, int size = 0, bool noCache = false)
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

	private async Task<IImage?> GetUserAvatarNoCacheAsync(ulong uin, int size)
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

	private async Task<IImage?> GetUserAvatarFromCacheAsync(ulong uin, int size)
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

	public Task<IImage?> GetGroupAvatarAsync(ulong uin, int size = 0, bool noCache = false)
	{
		if (!Directory.Exists(_groupsDirectory))
		{
			Directory.CreateDirectory(_groupsDirectory);
		}
		if (noCache)
		{
			return GetGroupAvatarNoCacheAsync(uin, size);
		}
		else
		{
			return GetGroupAvatarFromCacheAsync(uin, size);
		}
	}

	private async Task<IImage?> GetGroupAvatarNoCacheAsync(ulong uin, int size)
	{
		try
		{
			var response = await Shared.HttpClient.GetAsync($"https://p.qlogo.cn/gh/{uin}/{uin}/{size}");
			response.EnsureSuccessStatusCode();
			var bytes = await response.Content.ReadAsByteArrayAsync();
			var type = bytes.GetMediaType();
			if (type == MediaType.Unknown)
			{
				return null;
			}
			var directory = Path.Combine(_groupsDirectory, size.ToString());
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
			_logger.LogError(e, "Failed to get group {Uin}'s avatar of size {Size}.", uin, size);
			return null;
		}
	}

	private async Task<IImage?> GetGroupAvatarFromCacheAsync(ulong uin, int size)
	{
		try
		{
			var directory = Path.Combine(_groupsDirectory, size.ToString());
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
			var paths = Directory.GetFiles(directory, $"{uin}.*");
			var path = paths.FirstOrDefault();
			if (string.IsNullOrEmpty(path)
				|| !File.Exists(path))
			{
				return await GetGroupAvatarNoCacheAsync(uin, size);
			}
			return new Bitmap(path);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get group {Uin}'s avatar of size {Size}.", uin, size);
			return null;
		}
	}

	#endregion
}
