using AvaQQ.SDK;

namespace AvaQQ.Core;

internal class AppLifetimeController : IAppLifetimeController
{
	public CancellationTokenSource CancellationTokenSource { get; } = new();

	public event EventHandler? Stopping;

	public void Stop()
	{
		Stopping?.Invoke(this, EventArgs.Empty);
		CancellationTokenSource.Cancel();
	}
}
