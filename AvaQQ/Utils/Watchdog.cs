using System;
using System.Threading;

namespace AvaQQ.Utils;

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

	public void Dispose()
	{
		_timer.Dispose();
	}
}
