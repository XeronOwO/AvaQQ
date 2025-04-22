namespace AvaQQ.SDK;

/// <summary>
/// 应用生命周期控制器
/// </summary>
public interface IAppLifetimeController
{
	/// <summary>
	/// 当应用退出时触发取消令牌
	/// </summary>
	CancellationToken CancellationToken { get; }

	/// <summary>
	/// 停止应用
	/// </summary>
	void Stop();

	/// <summary>
	/// 停止应用时触发
	/// </summary>
	event EventHandler? Stopping;
}
