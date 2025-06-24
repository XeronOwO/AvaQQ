namespace AvaQQ.Core.Utils;

/// <summary>
/// 读写锁扩展
/// </summary>
public static class ReaderWriterLockSlimExtensions
{
	/// <summary>
	/// 可自动释放的读锁
	/// </summary>
	public readonly ref struct DisposableReaderLock : IDisposable
	{
		private readonly ReaderWriterLockSlim _lock;

		/// <summary>
		/// 创建一个可自动释放的读锁
		/// </summary>
		/// <param name="lock">读写锁</param>
		public DisposableReaderLock(ReaderWriterLockSlim @lock)
		{
			_lock = @lock;
			_lock.EnterReadLock();
		}

		/// <inheritdoc/>
		public readonly void Dispose()
		{
			_lock.ExitReadLock();
		}
	}

	/// <summary>
	/// 获取一个可自动释放的读锁
	/// </summary>
	/// <param name="lock">读写锁</param>
	public static DisposableReaderLock UseReadLock(this ReaderWriterLockSlim @lock)
	{
		return new DisposableReaderLock(@lock);
	}

	/// <summary>
	/// 可升级的可自动释放的读锁
	/// </summary>
	public readonly ref struct DisposableUpgradeableReaderLock : IDisposable
	{
		private readonly ReaderWriterLockSlim _lock;

		/// <summary>
		/// 创建一个可升级的可自动释放的读锁
		/// </summary>
		/// <param name="lock">读写锁</param>
		public DisposableUpgradeableReaderLock(ReaderWriterLockSlim @lock)
		{
			_lock = @lock;
			_lock.EnterUpgradeableReadLock();
		}

		/// <inheritdoc/>
		public readonly void Dispose()
		{
			_lock.ExitUpgradeableReadLock();
		}
	}

	/// <summary>
	/// 获取一个可升级的可自动释放的读锁
	/// </summary>
	/// <param name="lock">读写锁</param>
	public static DisposableUpgradeableReaderLock UseUpgradeableReadLock(this ReaderWriterLockSlim @lock)
	{
		return new DisposableUpgradeableReaderLock(@lock);
	}

	/// <summary>
	/// 可自动释放的写锁
	/// </summary>
	public readonly ref struct DisposableWriterLock : IDisposable
	{
		private readonly ReaderWriterLockSlim _lock;

		/// <summary>
		/// 创建一个可自动释放的写锁
		/// </summary>
		/// <param name="lock">读写锁</param>
		public DisposableWriterLock(ReaderWriterLockSlim @lock)
		{
			_lock = @lock;
			_lock.EnterWriteLock();
		}

		/// <inheritdoc/>
		public readonly void Dispose()
		{
			_lock.ExitWriteLock();
		}
	}

	/// <summary>
	/// 获取一个可自动释放的写锁
	/// </summary>
	/// <param name="lock">读写锁</param>
	public static DisposableWriterLock UseWriteLock(this ReaderWriterLockSlim @lock)
	{
		return new DisposableWriterLock(@lock);
	}
}
