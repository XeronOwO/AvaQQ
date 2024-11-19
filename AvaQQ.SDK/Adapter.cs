using AvaQQ.SDK.Logging;
using System;
using System.Threading.Tasks;

namespace AvaQQ.SDK;

/// <summary>
/// Protocol adapter.
/// </summary>
public abstract class Adapter : IDisposable
{
	/// <summary>
	/// Try to connect to the server.<br/>
	/// DO NOT call this method manually, unless you know what you are doing.
	/// </summary>
	/// <param name="timeout">Connection timeout.</param>
	/// <returns>Whether the connection is successful.</returns>
	public abstract Task<bool> ConnectAsync(TimeSpan timeout);

	/// <summary>
	/// Logs recorder.
	/// </summary>
	public abstract LogRecorder Logs { get; }

	/// <inheritdoc/>
	public abstract void Dispose();

	/// <summary>
	/// Get the QQ number of the current user.
	/// </summary>
	public abstract long Uin { get; }

	/// <summary>
	/// Get the name of the current user.
	/// </summary>
	public abstract Task<string> GetNameAsync();
}
