namespace AvaQQ.Core.Events;

/// <summary>
/// 公交车事件参数
/// </summary>
/// <typeparam name="TId">ID 类型</typeparam>
/// <param name="id">事件 ID</param>
public class BusEventArgs<TId>(TId id) : EventArgs where TId : IEventId
{
	/// <summary>
	/// 事件 ID
	/// </summary>
	public TId Id { get; } = id;
}

/// <summary>
/// 公交车事件参数
/// </summary>
/// <typeparam name="TId">ID 类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
/// <param name="id">事件 ID</param>
/// <param name="value">结果</param>
public class BusEventArgs<TId, TResult>(TId id, TResult value) : BusEventArgs<TId>(id) where TId : IEventId
{
	/// <summary>
	/// 结果
	/// </summary>
	public TResult Result { get; } = value;
}
