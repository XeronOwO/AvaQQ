using Avalonia.Media.Imaging;
using AvaQQ.Core.Events;
using AvaQQ.Core.Utils;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Numerics;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.CacheConfiguration>;

namespace AvaQQ.Core.Caches;

internal class AvatarCache : IAvatarCache
{
	private readonly IServiceProvider _serviceProvider;

	private readonly string _baseDirectory;

	private readonly string _userDirectory;

	private readonly string _groupDirectory;

	private readonly ILogger<AvatarCache> _logger;

	private readonly EventStation _events;

	public AvatarCache(IServiceProvider serviceProvider)
	{
		_baseDirectory = Path.Combine(Constants.RootDirectory, "cache", "avatar");
		_userDirectory = Path.Combine(_baseDirectory, "user");
		_groupDirectory = Path.Combine(_baseDirectory, "group");
		Directory.CreateDirectory(_baseDirectory);
		Directory.CreateDirectory(_userDirectory);
		Directory.CreateDirectory(_groupDirectory);

		CirculationInjectionDetector<AvatarCache>.Enter();

		_serviceProvider = serviceProvider;
		_logger = _serviceProvider.GetRequiredService<ILogger<AvatarCache>>();
		_events = _serviceProvider.GetRequiredService<EventStation>();

		CirculationInjectionDetector<AvatarCache>.Leave();

		_events.UserAvatar.OnDone += OnUserAvatar;
		_events.GroupAvatar.OnDone += OnGroupAvatar;
	}

	private struct Cache : IEqualityOperators<Cache, Cache, bool>
	{
		public required DateTime LastUpdateTime { get; set; }

		public required Bitmap? Avatar { get; set; }

		public readonly bool RequiresUpdate => Avatar is null || LastUpdateTime + Config.Instance.FriendAvatarExpiration < DateTime.Now;

		public static bool operator ==(Cache left, Cache right)
			=> left.Equals(right);

		public static bool operator !=(Cache left, Cache right)
			=> !left.Equals(right);

		public override readonly bool Equals(object? obj)
		{
			if (obj is not Cache info)
			{
				return false;
			}
			return LastUpdateTime == info.LastUpdateTime && Avatar == info.Avatar;
		}

		public override readonly int GetHashCode()
		{
			return HashCode.Combine(LastUpdateTime, Avatar);
		}
	}

	#region 用户

	private readonly ConcurrentDictionary<ulong, ConcurrentDictionary<int, Cache>> _userAvatars = [];

	public Bitmap? GetUserAvatar(ulong uin, int size = 0, bool forceUpdate = false)
	{
		var avatars = _userAvatars.GetOrAdd(uin, (_) => new());
		var avatar = avatars.GetOrAdd(size, (_) =>
		{
			return LoadUserAvatarFromFile(uin, size);
		});

		if (forceUpdate || avatar.RequiresUpdate)
		{
			_events.UserAvatar.Enqueue(new AvatarCacheId()
			{
				Uin = uin,
				Size = size,
			}, () => LoadUserAvatarFromRemoteAsync(uin, size));
		}

		return avatar.Avatar;
	}

	private Cache LoadUserAvatarFromFile(ulong uin, int size)
	{
		var directory = Path.Combine(_userDirectory, size.ToString());
		Directory.CreateDirectory(directory);
		var files = Directory.GetFiles(directory, $"{uin}.*");
		if (files.Length == 0)
		{
			return default;
		}

		var file = files.First();
		var lastUpdateTime = File.GetLastWriteTime(file);
		var bitmap = new Bitmap(file);
		return new()
		{
			LastUpdateTime = lastUpdateTime,
			Avatar = bitmap,
		};
	}

	private async Task<Bitmap?> LoadUserAvatarFromRemoteAsync(ulong uin, int size)
	{
		try
		{
			_logger.LogDebug("Downloading user {Uin}'s avatar of size {Size}.", uin, size);

			var response = await Shared.HttpClient.GetAsync($"https://q1.qlogo.cn/g?b=qq&nk={uin}&s={size}");
			response.EnsureSuccessStatusCode();
			var bytes = await response.Content.ReadAsByteArrayAsync();
			var type = bytes.GetMediaType();
			var file = Path.Combine(_userDirectory, size.ToString(), $"{uin}{type.GetFileExtension()}");
			await File.WriteAllBytesAsync(file, bytes);
			using var stream = new MemoryStream(bytes);
			return new Bitmap(stream);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get user {Uin}'s avatar of size {Size}.", uin, size);
			return null;
		}
	}

	private void OnUserAvatar(object? sender, BusEventArgs<AvatarCacheId, Bitmap?> e)
	{
		var avatars = _userAvatars.GetOrAdd(e.Id.Uin, (_) => new());
		avatars.AddOrUpdate(e.Id.Size, new Cache()
		{
			LastUpdateTime = DateTime.Now,
			Avatar = e.Result,
		}, (_, info) =>
		{
			info.LastUpdateTime = DateTime.Now;
			info.Avatar = e.Result;
			return info;
		});
	}

	#endregion

	#region 群聊

	private readonly ConcurrentDictionary<ulong, ConcurrentDictionary<int, Cache>> _groupAvatars = [];

	public Bitmap? GetGroupAvatar(ulong uin, int size = 0, bool forceUpdate = false)
	{
		var avatars = _groupAvatars.GetOrAdd(uin, (_) => new());
		var avatar = avatars.GetOrAdd(size, (_) =>
		{
			return LoadGroupAvatarFromFile(uin, size);
		});

		if (forceUpdate || avatar.RequiresUpdate)
		{
			_events.GroupAvatar.Enqueue(new AvatarCacheId()
			{
				Uin = uin,
				Size = size,
			}, () => LoadGroupAvatarFromRemoteAsync(uin, size));
		}

		return avatar.Avatar;
	}

	private Cache LoadGroupAvatarFromFile(ulong uin, int size)
	{
		var directory = Path.Combine(_groupDirectory, size.ToString());
		Directory.CreateDirectory(directory);
		var files = Directory.GetFiles(directory, $"{uin}.*");
		if (files.Length == 0)
		{
			return default;
		}

		var file = files.First();
		var lastUpdateTime = File.GetLastWriteTime(file);
		var bitmap = new Bitmap(file);
		return new()
		{
			LastUpdateTime = lastUpdateTime,
			Avatar = bitmap,
		};
	}

	private async Task<Bitmap?> LoadGroupAvatarFromRemoteAsync(ulong uin, int size)
	{
		try
		{
			_logger.LogDebug("Downloading group {Uin}'s avatar of size {Size}.", uin, size);

			var response = await Shared.HttpClient.GetAsync($"https://p.qlogo.cn/gh/{uin}/{uin}/{size}");
			response.EnsureSuccessStatusCode();
			var bytes = await response.Content.ReadAsByteArrayAsync();
			var type = bytes.GetMediaType();
			var file = Path.Combine(_groupDirectory, size.ToString(), $"{uin}{type.GetFileExtension()}");
			await File.WriteAllBytesAsync(file, bytes);
			using var stream = new MemoryStream(bytes);
			return new Bitmap(stream);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get group {Uin}'s avatar of size {Size}.", uin, size);
			return null;
		}
	}

	private void OnGroupAvatar(object? sender, BusEventArgs<AvatarCacheId, Bitmap?> e)
	{
		var avatars = _groupAvatars.GetOrAdd(e.Id.Uin, (_) => new());
		avatars.AddOrUpdate(e.Id.Size, new Cache()
		{
			LastUpdateTime = DateTime.Now,
			Avatar = e.Result,
		}, (_, info) =>
		{
			info.LastUpdateTime = DateTime.Now;
			info.Avatar = e.Result;
			return info;
		});
	}

	#endregion

	public void Clear()
	{
		_userAvatars.Clear();
		_groupAvatars.Clear();
	}
}
