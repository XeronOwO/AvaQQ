using AvaQQ.SDK;
using System;
using System.Threading;

namespace AvaQQ;

internal class LifetimeController : ILifetimeController
{
	public CancellationTokenSource CancellationTokenSource { get; } = new();

	public event EventHandler? Stopping;

	public void Stop()
	{
		Stopping?.Invoke(this, EventArgs.Empty);
		CancellationTokenSource.Cancel();
	}
}
