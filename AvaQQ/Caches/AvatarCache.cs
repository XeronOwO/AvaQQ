using Avalonia.Media.Imaging;
using AvaQQ.Events;
using AvaQQ.SDK;
using AvaQQ.SDK.Entities;
using AvaQQ.SDK.Utils;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Configurations.CacheConfiguration>;

#pragma warning disable IDE0079
#pragma warning disable CA1868

namespace AvaQQ.Caches;

public class AvatarCache(ILogger<AvatarCache> logger, IAppLifetime lifetime, AvaQQEvents events)
	: Cache<AvatarId, AvatarCache.Value>(Config.Instance.AvatarExpiration), IDisposable
{
	private readonly HttpClient _httpClient = new();

	public class Value : IUpdateTime
	{
		public DateTimeOffset UpdateTime { get; set; } = DateTimeOffset.MinValue;

		public Bitmap? Avatar { get; set; } = null;

		public byte[] Hash { get; set; } = [];
	}

	private readonly HashSet<AvatarId> _initialized = [];

	private readonly ReaderWriterLockSlim _lock = new();

	protected override void OnUpdateRequested(AvatarId key, Value? value)
	{
		using var lock1 = _lock.UseUpgradeableReadLock();

		if (!_initialized.Contains(key))
		{
			using var lock2 = _lock.UseWriteLock();
			if (!_initialized.Contains(key))
			{
				_initialized.Add(key);
				Task.Run(() => FetchFromDiskThenUrlAsync(key, lifetime.Token), lifetime.Token);
			}
			return;
		}

		events.OnAvatarFetched.Invoke(key, () => FetchFromUrlAsync(key, lifetime.Token));
	}

	private void UpdateCache(AvatarId key, DateTimeOffset time, byte[] bytes)
	{
		var hash = MD5.HashData(bytes);

		AvatarChangedInfo? info = null;
		AddOrUpdate(key, _ =>
		{
			using var stream = new MemoryStream(bytes);
			var avatar = new Bitmap(stream);
			info = new(null, avatar);

			return new Value
			{
				UpdateTime = time,
				Avatar = avatar,
				Hash = hash
			};
		}, (_, value) =>
		{
			if (value.UpdateTime >= time)
			{
				return value;
			}
			if (value.Hash.SequenceEqual(hash))
			{
				value.UpdateTime = time;
				return value;
			}

			using var stream = new MemoryStream(bytes);
			var newAvatar = new Bitmap(stream);
			info = new(value.Avatar, newAvatar);

			value.UpdateTime = time;
			value.Avatar = newAvatar;
			value.Hash = hash;
			return value;
		});

		if (info is not null)
		{
			events.OnAvatarChanged.Invoke(key, info.Value);
		}
	}

	private async Task FetchFromDiskThenUrlAsync(AvatarId key, CancellationToken token = default)
	{
		try
		{
			logger.LogInformation("Fetching {Category} {Uin}'s avatar of size {Size} from disk", key.Category.GetLowercaseName(), key.Uin, key.Size);

			var dir = Path.Combine(Constants.RootDirectory, "avatar", key.Category.GetLowercaseName(), key.Size.ToString());
			var files = Directory.GetFiles(dir, $"{key.Uin}.*");
			if (files.Length == 0)
			{
				logger.LogInformation("No cached avatar found for {Category} {Uin}'s avatar of size {Size}", key.Category.GetLowercaseName(), key.Uin, key.Size);
				events.OnAvatarFetched.Invoke(key, () => FetchFromUrlAsync(key, lifetime.Token));
				return;
			}

			var file = files.First();
			var time = File.GetLastWriteTime(file);
			var bytes = await File.ReadAllBytesAsync(file, token);
			UpdateCache(key, time, bytes);
			logger.LogInformation("Fetched {Category} {Uin}'s avatar of size {Size} from disk", key.Category.GetLowercaseName(), key.Uin, key.Size);

			events.OnAvatarFetched.Invoke(key, () => FetchFromUrlAsync(key, lifetime.Token));
		}
		catch (OperationCanceledException)
		{
			logger.LogInformation("Operation cancelled: fetching {Category} {Uin}'s avatar of size {Size} from disk", key.Category.GetLowercaseName(), key.Uin, key.Size);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to fetch {Category} {Uin}'s avatar of size {Size} from disk", key.Category.GetLowercaseName(), key.Uin, key.Size);
			events.OnAvatarFetched.Invoke(key, () => FetchFromUrlAsync(key, lifetime.Token));
		}
	}

	private async Task<byte[]?> FetchFromUrlAsync(AvatarId key, CancellationToken token = default)
	{
		try
		{
			logger.LogInformation("Fetching {Category} {Uin}'s avatar of size {Size} from url", key.Category.GetLowercaseName(), key.Uin, key.Size);

			var response = await _httpClient.GetAsync(key.Url, token);
			response.EnsureSuccessStatusCode();
			var bytes = await response.Content.ReadAsByteArrayAsync(token);
			UpdateCache(key, DateTimeOffset.Now, bytes);
			await SaveAsync(key, bytes, token);
			logger.LogInformation("Fetched {Category} {Uin}'s avatar of size {Size} from url", key.Category.GetLowercaseName(), key.Uin, key.Size);
			return bytes;
		}
		catch (OperationCanceledException)
		{
			logger.LogInformation("Operation cancelled: fetching {Category} {Uin}'s avatar of size {Size} from url", key.Category.GetLowercaseName(), key.Uin, key.Size);
			return null;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to fetch {Category} {Uin}'s avatar of size {Size} from url", key.Category.GetLowercaseName(), key.Uin, key.Size);
			return null;
		}
	}

	private async Task SaveAsync(AvatarId key, byte[] bytes, CancellationToken token = default)
	{
		try
		{
			var dir = Path.Combine(Constants.RootDirectory, "avatar", key.Category.GetLowercaseName(), key.Size.ToString());
			Directory.CreateDirectory(dir);
			var path = Path.Combine(dir, $"{key.Uin}{bytes.GetMediaType().GetFileExtension()}");
			await File.WriteAllBytesAsync(path, bytes, token);
		}
		catch (OperationCanceledException)
		{
			logger.LogInformation("Operation cancelled: saving {Category} {Uin}'s avatar of size {Size}", key.Category.GetLowercaseName(), key.Uin, key.Size);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to save {Category} {Uin}'s avatar of size {Size}", key.Category.GetLowercaseName(), key.Uin, key.Size);
		}
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_httpClient.Dispose();
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
}
