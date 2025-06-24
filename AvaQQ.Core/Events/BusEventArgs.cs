namespace AvaQQ.Core.Events;

/// <summary>
/// 公交车事件参数
/// </summary>
public class BusEventArgs : EventArgs
{
}

/// <summary>
/// 公交车事件参数
/// </summary>
/// <typeparam name="TResult">结果类型</typeparam>
/// <param name="value">结果</param>
public class BusEventArgs<TResult>(TResult value) : BusEventArgs
{
	/// <summary>
	/// 结果
	/// </summary>
	public TResult Result { get; } = value;
}

/// <summary>
/// 公交车事件参数
/// </summary>
/// <typeparam name="TId">ID 类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
/// <param name="id">事件 ID</param>
/// <param name="value">结果</param>
public class BusEventArgs<TId, TResult>(TId id, TResult value)
	: BusEventArgs<TResult>(value) where TId : IEquatable<TId>
{
	/// <summary>
	/// 事件 ID
	/// </summary>
	public TId Id { get; } = id;
}
