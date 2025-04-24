using System.Collections.Concurrent;

namespace AvaQQ.Core.Events;

/// <summary>
/// 事件公交车
/// </summary>
/// <typeparam name="TId">ID 类型</typeparam>
public class EventBus<TId> where TId : IEventId
{
	private readonly ConcurrentDictionary<TId, Task> _tasks = [];

	/// <summary>
	/// 加入事件队列<br/>
	/// 当车上有相同 ID 的事件时，不会加入队列，并返回 false
	/// </summary>
	/// <param name="id">事件 ID</param>
	/// <param name="task">任务</param>
	/// <returns>是否成功加入队列</returns>
	public bool Enqueue(TId id, Task task)
	{
		var result = false;
		_tasks.GetOrAdd(id, (_) =>
		{
			task.ContinueWith((t) =>
			{
				Done(id);
			});

			result = true;
			return task;
		});

		return result;
	}

	/// <summary>
	/// 当事件完成时触发
	/// </summary>
	public event EventHandler<BusEventArgs<TId>>? OnDone;

	private void Done(TId id)
	{
		_tasks.Remove(id, out var _);

		OnDone?.Invoke(this, new(id));
	}

	/// <summary>
	/// 对于一些非异步事件，不需要加入队列，直接触发完成事件，也就是直接调用 <see cref="OnDone"/>
	/// </summary>
	/// <param name="id">事件 ID</param>
	public void DoneManually(TId id)
	{
		OnDone?.Invoke(this, new(id));
	}
}

/// <summary>
/// 事件公交车
/// </summary>
/// <typeparam name="TId">ID 类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
public class EventBus<TId, TResult> where TId : IEventId
{
	private readonly ConcurrentDictionary<TId, Task<TResult>> _tasks = [];

	/// <summary>
	/// 加入事件队列<br/>
	/// 当车上有相同 ID 的事件时，不会加入队列，并返回 false
	/// </summary>
	/// <param name="id">事件 ID</param>
	/// <param name="taskFactory">任务</param>
	/// <returns>是否成功加入队列</returns>
	public bool Enqueue(TId id, Func<Task<TResult>> taskFactory)
	{
		var result = false;
		_tasks.GetOrAdd(id, (_) =>
		{
			var task = taskFactory();
			task.ContinueWith((t) =>
			{
				Done(id, t.Result);
			});

			result = true;
			return task;
		});

		return result;
	}

	/// <summary>
	/// 当事件完成时触发
	/// </summary>
	public event EventHandler<BusEventArgs<TId, TResult>>? OnDone;

	private void Done(TId id, TResult result)
	{
		_tasks.Remove(id, out var _);

		OnDone?.Invoke(this, new(id, result));
	}

	/// <summary>
	/// 对于一些非异步事件，不需要加入队列，直接触发完成事件，也就是直接调用 <see cref="OnDone"/>
	/// </summary>
	/// <param name="id">事件 ID</param>
	/// <param name="result">结果</param>
	public void DoneManually(TId id, TResult result)
	{
		OnDone?.Invoke(this, new(id, result));
	}
}
