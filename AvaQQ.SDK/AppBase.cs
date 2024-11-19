using Avalonia;

namespace AvaQQ.SDK;

/// <summary>
/// Represents the base class of the application.
/// </summary>
public abstract class AppBase : Application
{
	/// <summary>
	/// Gets the adapter of the application.
	/// </summary>
	public abstract Adapter? Adapter { get; }

	/// <summary>
	/// Gets the ensured adapter of the application.
	/// </summary>
	public abstract Adapter EnsuredAdapter { get; }
}
