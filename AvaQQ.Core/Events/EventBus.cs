using AvaQQ.Core.Utils;
using System.Collections.Concurrent;

namespace AvaQQ.Core.Events;

/// <summary>
/// 事件公交车
/// </summary>
/// <typeparam name="TResult">结果类型</typeparam>
public class EventBus<TResult>
{
	private readonly ReaderWriterLockSlim _lock = new();

	private Task<TResult>? _task;

	/// <summary>
	/// 加入事件队列<br/>
	/// 当车上有相同 ID 的事件时，不会加入队列，并返回 false
	/// </summary>
	/// <param name="taskFactory">任务</param>
	/// <returns>是否成功加入队列</returns>
	public bool Invoke(Func<Task<TResult>> taskFactory)
	{
		using var _ = _lock.UseUpgradeableReadLock();
		if (_task != null)
		{
			return false;
		}

		using var __ = _lock.UseWriteLock();
		if (_task != null)
		{
			return false;
		}

		_task = taskFactory();
		_task.ContinueWith(t =>
		{
			Invoke(t.Result);
			return t.Result;
		});
		return true;
	}

	/// <summary>
	/// 对于一些非异步事件，不需要加入队列，直接触发完成事件
	/// </summary>
	/// <param name="result">结果</param>
	public void Invoke(TResult result)
		=> Event?.Invoke(this, new(result));

	/// <summary>
	/// 当事件完成时触发
	/// </summary>
	private event EventHandler<BusEventArgs<TResult>>? Event;

	/// <summary>
	/// 订阅事件
	/// </summary>
	/// <param name="handler">处理器</param>
	public void Subscribe(EventHandler<BusEventArgs<TResult>> handler)
		=> Event += handler;

	/// <summary>
	/// 取消订阅事件
	/// </summary>
	/// <param name="handler">处理器</param>
	public void Unsubscribe(EventHandler<BusEventArgs<TResult>> handler)
		=> Event -= handler;
}

/// <summary>
/// 事件公交车
/// </summary>
/// <typeparam name="TId">ID 类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
public class EventBus<TId, TResult> where TId : IEquatable<TId>
{
	private readonly ConcurrentDictionary<TId, Task<TResult>> _tasks = [];

	/// <summary>
	/// 加入事件队列<br/>
	/// 当车上有相同 ID 的事件时，不会加入队列，并返回 false
	/// </summary>
	/// <param name="id">事件 ID</param>
	/// <param name="taskFactory">任务</param>
	/// <returns>是否成功加入队列</returns>
	public bool Invoke(TId id, Func<Task<TResult>> taskFactory)
	{
		var result = false;
		_tasks.GetOrAdd(id, (_) =>
		{
			var task = taskFactory();
			result = true;
			return task.ContinueWith(t =>
			{
				Invoke(id, t.Result);
				return t.Result;
			});
		});

		return result;
	}

	/// <summary>
	/// 对于一些非异步事件，不需要加入队列，直接触发完成事件
	/// </summary>
	/// <param name="id">事件 ID</param>
	/// <param name="result">结果</param>
	public void Invoke(TId id, TResult result)
		=> Event?.Invoke(this, new(id, result));

	/// <summary>
	/// 当事件完成时触发
	/// </summary>
	private event EventHandler<BusEventArgs<TId, TResult>>? Event;

	/// <summary>
	/// 订阅事件
	/// </summary>
	/// <param name="handler">处理器</param>
	public void Subscribe(EventHandler<BusEventArgs<TId, TResult>> handler)
		=> Event += handler;

	/// <summary>
	/// 取消订阅事件
	/// </summary>
	/// <param name="handler">处理器</param>
	public void Unsubscribe(EventHandler<BusEventArgs<TId, TResult>> handler)
		=> Event -= handler;
}
