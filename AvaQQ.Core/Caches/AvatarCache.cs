using Avalonia.Media.Imaging;
using AvaQQ.Core.Events;
using AvaQQ.Core.Utils;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Security.Cryptography;
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

		CirculationInjectionDetector<AvatarCache>.Enter();

		_serviceProvider = serviceProvider;
		_logger = _serviceProvider.GetRequiredService<ILogger<AvatarCache>>();
		_events = _serviceProvider.GetRequiredService<EventStation>();

		CirculationInjectionDetector<AvatarCache>.Leave();

		_events.OnUserAvatarFetched.Subscribe(OnUserAvatarFetched);
		_events.OnGroupAvatarFetched.Subscribe(OnGroupAvatarFetched);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_events.OnUserAvatarFetched.Unsubscribe(OnUserAvatarFetched);
			_events.OnGroupAvatarFetched.Unsubscribe(OnGroupAvatarFetched);

			if (disposing)
			{
			}

			disposedValue = true;
		}
	}

	~AvatarCache()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion

	private record struct Cache(DateTime Time, Bitmap? Avatar, byte[] Hash)
	{
		public readonly bool RequiresUpdate => DateTime.Now > Time + Config.Instance.AvatarExpiration;

		public static readonly Cache Default = new(DateTime.MinValue, null, []);
	}

	#region 用户

	private readonly ConcurrentDictionary<ulong, ConcurrentDictionary<int, Cache>> _userCache = [];

	public Bitmap? GetUserAvatar(ulong uin, int size = 0, bool forceUpdate = false)
	{
		var caches = _userCache.GetOrAdd(uin, _ => new());
		var cache = caches.GetOrAdd(size, _ => LoadUserAvatarCacheFromLocal(uin, size));

		if (forceUpdate || cache.RequiresUpdate)
		{
			_events.OnUserAvatarFetched.Invoke(
				new AvatarId(uin, size),
				() => FetchUserAvatarFromUrlAsync(uin, size)
				);
		}

		return cache.Avatar;
	}

	private Cache LoadUserAvatarCacheFromLocal(ulong uin, int size)
	{
		var directory = Path.Combine(_userDirectory, size.ToString());
		Directory.CreateDirectory(directory);
		var files = Directory.GetFiles(directory, $"{uin}.*");
		if (files.Length == 0)
		{
			return Cache.Default;
		}

		var file = files.First();
		var time = File.GetLastWriteTime(file);
		var bytes = File.ReadAllBytes(file);
		var hash = MD5.HashData(bytes);
		using var stream = new MemoryStream(bytes);
		var bitmap = new Bitmap(stream);
		return new(time, bitmap, hash);
	}

	private async Task<byte[]?> FetchUserAvatarFromUrlAsync(ulong uin, int size)
	{
		try
		{
			_logger.LogDebug("Downloading user {Uin}'s avatar of size {Size}.", uin, size);

			var response = await Shared.HttpClient.GetAsync($"https://q1.qlogo.cn/g?b=qq&nk={uin}&s={size}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsByteArrayAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to fetch user {Uin}'s avatar of size {Size}.", uin, size);
			return null;
		}
	}

	private void OnUserAvatarFetched(object? sender, BusEventArgs<AvatarId, byte[]?> e)
	{
		var caches = _userCache.GetOrAdd(e.Id.Uin, _ => new());

		Bitmap? oldAvatar = null;
		Bitmap? newAvatar = null;
		caches.AddOrUpdate(e.Id.Size, _ => throw new InvalidOperationException(), (_, oldCache) =>
		{
			var time = DateTime.Now;
			if (e.Result is not { } bytes)
			{
				return new(time, oldCache.Avatar, oldCache.Hash);
			}

			var hash = MD5.HashData(bytes);
			if (hash.SequenceEqual(oldCache.Hash))
			{
				return new(time, oldCache.Avatar, oldCache.Hash);
			}

			var directory = Path.Combine(_userDirectory, e.Id.Size.ToString());
			Directory.CreateDirectory(directory);
			var path = Path.Combine(directory, $"{e.Id.Uin}{bytes.GetMediaType().GetFileExtension()}");
			File.WriteAllBytes(path, bytes);
			using var stream = new MemoryStream(bytes);
			var avatar = new Bitmap(stream);
			oldAvatar = oldCache.Avatar;
			newAvatar = avatar;
			return new(time, avatar, hash);
		});

		if (oldAvatar != newAvatar && newAvatar != null)
		{
			_events.OnUserAvatarChanged.Invoke(e.Id, new AvatarChangedInfo(oldAvatar, newAvatar));
		}
	}

	#endregion

	#region 群聊

	private readonly ConcurrentDictionary<ulong, ConcurrentDictionary<int, Cache>> _groupCache = [];

	public Bitmap? GetGroupAvatar(ulong uin, int size = 0, bool forceUpdate = false)
	{
		var caches = _groupCache.GetOrAdd(uin, (_) => new());
		var cache = caches.GetOrAdd(size, _ => LoadGroupAvatarCacheFromLocal(uin, size));

		if (forceUpdate || cache.RequiresUpdate)
		{
			_events.OnGroupAvatarFetched.Invoke(
				new AvatarId(uin, size),
				() => FetchGroupAvatarFromUrlAsync(uin, size)
				);
		}

		return cache.Avatar;
	}

	private Cache LoadGroupAvatarCacheFromLocal(ulong uin, int size)
	{
		var directory = Path.Combine(_groupDirectory, size.ToString());
		Directory.CreateDirectory(directory);
		var files = Directory.GetFiles(directory, $"{uin}.*");
		if (files.Length == 0)
		{
			return Cache.Default;
		}

		var file = files.First();
		var time = File.GetLastWriteTime(file);
		var bytes = File.ReadAllBytes(file);
		var hash = MD5.HashData(bytes);
		using var stream = new MemoryStream(bytes);
		var bitmap = new Bitmap(stream);
		return new(time, bitmap, hash);
	}

	private async Task<byte[]?> FetchGroupAvatarFromUrlAsync(ulong uin, int size)
	{
		try
		{
			_logger.LogDebug("Downloading group {Uin}'s avatar of size {Size}.", uin, size);

			var response = await Shared.HttpClient.GetAsync($"https://p.qlogo.cn/gh/{uin}/{uin}/{size}");
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsByteArrayAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to fetch group {Uin}'s avatar of size {Size}.", uin, size);
			return null;
		}
	}

	private void OnGroupAvatarFetched(object? sender, BusEventArgs<AvatarId, byte[]?> e)
	{
		var caches = _groupCache.GetOrAdd(e.Id.Uin, (_) => new());

		Bitmap? oldAvatar = null;
		Bitmap? newAvatar = null;
		caches.AddOrUpdate(e.Id.Size, _ => throw new InvalidOperationException(), (_, oldCache) =>
		{
			var time = DateTime.Now;
			if (e.Result is not { } bytes)
			{
				return new(time, oldCache.Avatar, oldCache.Hash);
			}

			var hash = MD5.HashData(bytes);
			if (hash.SequenceEqual(oldCache.Hash))
			{
				return new(time, oldCache.Avatar, oldCache.Hash);
			}

			var directory = Path.Combine(_groupDirectory, e.Id.Size.ToString());
			Directory.CreateDirectory(directory);
			var path = Path.Combine(directory, $"{e.Id.Uin}{bytes.GetMediaType().GetFileExtension()}");
			File.WriteAllBytes(path, bytes);
			using var stream = new MemoryStream(bytes);
			var avatar = new Bitmap(stream);
			oldAvatar = oldCache.Avatar;
			newAvatar = avatar;
			return new(time, avatar, hash);
		});

		if (oldAvatar != newAvatar && newAvatar != null)
		{
			_events.OnGroupAvatarChanged.Invoke(e.Id, new AvatarChangedInfo(oldAvatar, newAvatar));
		}
	}

	#endregion

	public void Clear()
	{
		_userCache.Clear();
		_groupCache.Clear();
	}
}
