using AvaQQ.SDK;

namespace AvaQQ.Core;

internal class AppLifetimeController : IAppLifetimeController
{
	private readonly CancellationTokenSource _cts = new();

	public CancellationToken CancellationToken => _cts.Token;

	public event EventHandler? Stopping;

	public void Stop()
	{
		Stopping?.Invoke(this, EventArgs.Empty);
		_cts.Cancel();
	}
}
