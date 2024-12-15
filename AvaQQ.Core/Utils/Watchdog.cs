namespace AvaQQ.Core.Utils;

internal class Watchdog(TimerCallback callback) : IDisposable
{
	private readonly Timer _timer = new(callback);

	private TimeSpan _dueTime = Timeout.InfiniteTimeSpan;

	public void Start(TimeSpan dueTime)
	{
		_timer.Change(dueTime, Timeout.InfiniteTimeSpan);
		_dueTime = dueTime;
	}

	public void Feed()
	{
		_timer.Change(_dueTime, Timeout.InfiniteTimeSpan);
	}

	public void Stop()
	{
		_timer.Change(Timeout.Infinite, Timeout.Infinite);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_timer.Dispose();
			}

			disposedValue = true;
		}
	}

	~Watchdog()
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
