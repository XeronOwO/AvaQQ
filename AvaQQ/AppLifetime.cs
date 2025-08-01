using AvaQQ.SDK;

namespace AvaQQ;

internal class AppLifetime : IAppLifetime
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
