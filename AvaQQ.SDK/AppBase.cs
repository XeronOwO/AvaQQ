using Avalonia;

namespace AvaQQ.SDK;

public abstract class AppBase : Application
{
	public abstract Adapter? Adapter { get; }

	public abstract Adapter EnsuredAdapter { get; }
}
