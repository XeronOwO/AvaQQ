using System;
using System.Threading;

namespace AvaQQ.SDK;

/// <summary>
/// Represents a controller that manages the lifetime of the application.
/// </summary>
public interface ILifetimeController
{
	/// <summary>
	/// Gets the cancellation token source that is used to stop the application.<br/>
	/// DO NOT call <see cref="CancellationTokenSource.Cancel()"/> directly, use <see cref="Stop"/> instead.
	/// </summary>
	[Obsolete("DO NOT call CancellationTokenSource.Cancel() directly, use Stop instead.")]
	CancellationTokenSource CancellationTokenSource { get; }

	/// <summary>
	/// Stops the application.
	/// </summary>
	void Stop();
}
