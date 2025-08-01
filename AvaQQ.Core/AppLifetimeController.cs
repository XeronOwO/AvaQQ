using AvaQQ.SDK;

namespace AvaQQ.Core;

internal class AppLifetimeController : IAppLifetime
{
	private readonly CancellationTokenSource _cts = new();

	public CancellationToken Token => _cts.Token;

	public event EventHandler? OnShutdown;

	public void Shutdown()
	{
		OnShutdown?.Invoke(this, EventArgs.Empty);
		_cts.Cancel();
	}
}
