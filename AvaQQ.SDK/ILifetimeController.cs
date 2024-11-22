using System;
using System.Threading;

namespace AvaQQ.SDK;

/// <summary>
/// 应用生命周期控制器
/// </summary>
public interface ILifetimeController
{
	/// <summary>
	/// 获取一个 CancellationTokenSource，用于停止应用。<br/>
	/// 请勿直接调用 <see cref="CancellationTokenSource.Cancel()"/>，请使用 <see cref="Stop"/>。
	/// </summary>
	CancellationTokenSource CancellationTokenSource { get; }

	/// <summary>
	/// 停止应用
	/// </summary>
	void Stop();

	/// <summary>
	/// 停止应用时触发
	/// </summary>
	event EventHandler? Stopping;
}
