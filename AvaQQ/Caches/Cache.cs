using AvaQQ.SDK.Entities;
using AvaQQ.SDK.Utils;

namespace AvaQQ.Caches;

public abstract class Cache<TKey, TValue>(TimeSpan expiration)
	where TKey : notnull
	where TValue : class, IUpdateTime
{
	private readonly Dictionary<TKey, TValue> _caches = [];

	private readonly ReaderWriterLockSlim _lock = new();

	protected abstract void OnUpdateRequested(TKey key, TValue? value);

	public TValue? Get(TKey key, bool forceUpdate)
	{
		using var @lock = _lock.UseReadLock();
		if (!_caches.TryGetValue(key, out var value))
		{
			OnUpdateRequested(key, default);
			return default;
		}

		if (forceUpdate || DateTimeOffset.Now >= value.UpdateTime + expiration)
		{
			OnUpdateRequested(key, value);
		}

		return value;
	}

	protected void AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
	{
		using var @lock = _lock.UseWriteLock();
		if (!_caches.TryGetValue(key, out var value))
		{
			_caches[key] = addValueFactory(key);
		}
		else
		{
			_caches[key] = updateValueFactory(key, value);
		}
	}
}
