using Avalonia.Media.Imaging;
using AvaQQ.Core.Entities;
using AvaQQ.Core.Events;
using AvaQQ.Core.Utils;
using AvaQQ.SDK;
using AvaQQ.SDK.Utils;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.CacheConfiguration>;

namespace AvaQQ.Core.Contexts;

internal class AvatarCacheValue : IUpdateTime
{
	public DateTimeOffset UpdateTime { get; set; } = DateTimeOffset.MinValue;

	public Bitmap? Avatar { get; set; } = null;

	public byte[] Hash { get; set; } = [];
}

internal class AvatarCache(EventStation events, IAppLifetime lifetime, ILogger<AvatarCache> logger)
	: Cache<AvatarId, AvatarCacheValue>(Config.Instance.AvatarExpiration)
{
	private readonly HashSet<AvatarId> _initialized = [];

	private readonly ReaderWriterLockSlim _lock = new();

	protected override void OnUpdateRequested(AvatarId key, AvatarCacheValue? value)
	{
		using var lock1 = _lock.UseUpgradeableReadLock();

		if (!_initialized.Contains(key))
		{
			using var lock2 = _lock.UseWriteLock();
			if (!_initialized.Contains(key))
			{
				_initialized.Add(key);
				Task.Run(() => LoadFromDiskAsync(key, lifetime.Token));
			}
		}

		Task.Run(() => FetchFromUrlAsync(key, lifetime.Token));
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

			return new AvatarCacheValue
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

	private async Task LoadFromDiskAsync(AvatarId key, CancellationToken token = default)
	{
		try
		{
			logger.LogInformation("Loading {Category} {Uin}'s avatar of size {Size} from disk.", key.Category.GetName(), key.Uin, key.Size);

			var dir = Path.Combine(Constants.RootDirectory, "avatar", key.Category.GetName(), key.Size.ToString());
			var files = Directory.GetFiles(dir, $"{key.Uin}.*");
			if (files.Length == 0)
			{
				_ = FetchFromUrlAsync(key, lifetime.Token);
				return;
			}

			var file = files.First();
			var time = File.GetLastWriteTime(file);
			var bytes = await File.ReadAllBytesAsync(file, token);
			UpdateCache(key, time, bytes);

			_ = FetchFromUrlAsync(key, lifetime.Token);
		}
		catch (OperationCanceledException)
		{
			logger.LogInformation("Operation cancelled: loading {Category} {Uin}'s avatar of size {Size} from disk.", key.Category.GetName(), key.Uin, key.Size);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to load {Category} {Uin}'s avatar of size {Size} from disk.", key.Category.GetName(), key.Uin, key.Size);

			_ = FetchFromUrlAsync(key, lifetime.Token);
		}
	}

	public async Task FetchFromUrlAsync(AvatarId key, CancellationToken token = default)
	{
		try
		{
			logger.LogDebug("Fetching {Category} {Uin}'s avatar of size {Size} from url.", key.Category.GetName(), key.Uin, key.Size);

			var response = await Shared.HttpClient.GetAsync(key.Url, token);
			response.EnsureSuccessStatusCode();
			var bytes = await response.Content.ReadAsByteArrayAsync(token);
			UpdateCache(key, DateTimeOffset.Now, bytes);
			await SaveAsync(key, bytes, token);
		}
		catch (OperationCanceledException)
		{
			logger.LogInformation("Operation cancelled: fetching {Category} {Uin}'s avatar of size {Size} from url.", key.Category.GetName(), key.Uin, key.Size);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to fetch {Category} {Uin}'s avatar of size {Size} from url.", key.Category.GetName(), key.Uin, key.Size);
		}
	}

	private async Task SaveAsync(AvatarId key, byte[] bytes, CancellationToken token = default)
	{
		try
		{
			var dir = Path.Combine(Constants.RootDirectory, "avatar", key.Category.GetName(), key.Size.ToString());
			Directory.CreateDirectory(dir);
			var path = Path.Combine(dir, $"{key.Uin}{bytes.GetMediaType().GetFileExtension()}");
			await File.WriteAllBytesAsync(path, bytes, token);
		}
		catch (OperationCanceledException)
		{
			logger.LogInformation("Operation cancelled: saving {Category} {Uin}'s avatar of size {Size}.", key.Category.GetName(), key.Uin, key.Size);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to save {Category} {Uin}'s avatar of size {Size}.", key.Category.GetName(), key.Uin, key.Size);
		}
	}
}
