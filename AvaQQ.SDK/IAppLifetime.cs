namespace AvaQQ.SDK;

public interface IAppLifetime
{
	CancellationToken Token { get; }

	void Shutdown();

	event EventHandler? OnShutdown;
}
