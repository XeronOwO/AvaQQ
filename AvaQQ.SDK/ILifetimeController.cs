using System.Threading;

namespace AvaQQ.SDK;

/// <summary>
/// Represents a controller that manages the lifetime of the application.
/// </summary>
public interface ILifetimeController
{
	/// <summary>
	/// Gets the cancellation token source that is used to stop the application.
	/// </summary>
	CancellationTokenSource CancellationTokenSource { get; }
}
