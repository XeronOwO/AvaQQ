namespace AvaQQ.SDK.Utils;

public static class ReaderWriterLockSlimExtensions
{
	public readonly ref struct DisposableReaderLock : IDisposable
	{
		private readonly ReaderWriterLockSlim _lock;

		public DisposableReaderLock(ReaderWriterLockSlim @lock)
		{
			_lock = @lock;
			_lock.EnterReadLock();
		}

		public readonly void Dispose()
		{
			_lock.ExitReadLock();
		}
	}

	public static DisposableReaderLock UseReadLock(this ReaderWriterLockSlim @lock)
		=> new(@lock);

	public readonly ref struct DisposableUpgradeableReaderLock : IDisposable
	{
		private readonly ReaderWriterLockSlim _lock;

		public DisposableUpgradeableReaderLock(ReaderWriterLockSlim @lock)
		{
			_lock = @lock;
			_lock.EnterUpgradeableReadLock();
		}

		public readonly void Dispose()
		{
			_lock.ExitUpgradeableReadLock();
		}
	}

	public static DisposableUpgradeableReaderLock UseUpgradeableReadLock(this ReaderWriterLockSlim @lock)
		=> new(@lock);

	public readonly ref struct DisposableWriterLock : IDisposable
	{
		private readonly ReaderWriterLockSlim _lock;

		public DisposableWriterLock(ReaderWriterLockSlim @lock)
		{
			_lock = @lock;
			_lock.EnterWriteLock();
		}

		public readonly void Dispose()
		{
			_lock.ExitWriteLock();
		}
	}

	public static DisposableWriterLock UseWriteLock(this ReaderWriterLockSlim @lock)
		=> new(@lock);
}
