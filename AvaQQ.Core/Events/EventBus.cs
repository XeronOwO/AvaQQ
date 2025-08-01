using AvaQQ.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace AvaQQ.Core.Events;

/// <summary>
/// 事件公交车接口
/// </summary>
public interface IEventBus
{
	internal bool CheckSubscriptionsOnExit();
}

/// <summary>
/// 事件公交车
/// </summary>
/// <typeparam name="TResult">结果类型</typeparam>
/// <param name="serviceProvider">服务提供者</param>
/// <param name="name">名称</param>
/// <param name="ensureUIThread">确保调用事件处理器时使用的是UI线程</param>
public class EventBus<TResult>(IServiceProvider serviceProvider, string name) : IEventBus
{
	private readonly ILogger<EventBus<TResult>> _logger = serviceProvider.GetRequiredService<ILogger<EventBus<TResult>>>();

	private readonly ReaderWriterLockSlim _taskLock = new();

	private Task? _wrappedTask;

	/// <summary>
	/// 加入事件队列<br/>
	/// 当车上有相同 ID 的事件时，不会加入队列，并返回 false
	/// </summary>
	/// <param name="taskFactory">任务</param>
	/// <returns>是否成功加入队列</returns>
	public bool Invoke(Func<Task<TResult>> taskFactory)
	{
		using var lock1 = _taskLock.UseUpgradeableReadLock();
		if (_wrappedTask != null)
		{
			_logger.LogTrace("[{Name}] Task already exists.", name);
			return false;
		}

		using var lock2 = _taskLock.UseWriteLock();
		if (_wrappedTask != null)
		{
			_logger.LogTrace("[{Name}] Task already exists.", name);
			return false;
		}

		var task = taskFactory();
		_wrappedTask = task.ContinueWith(t =>
		{
			Invoke(t.Result);
			using var lock3 = _taskLock.UseWriteLock();
			_logger.LogTrace("[{Name}] Task {TaskId} done.", name, task.Id);
			_wrappedTask = null;
		});
		_logger.LogTrace("[{Name}] Task {TaskId} enqueued.", name, task.Id);
		return true;
	}

	private readonly ReaderWriterLockSlim _handlerLock = new();

	private readonly SortedDictionary<int, EventHandler<BusEventArgs<TResult>>> _handlers = [];

	/// <summary>
	/// 对于一些非异步事件，不需要加入队列，直接触发完成事件
	/// </summary>
	/// <param name="result">结果</param>
	public void Invoke(TResult result)
	{
		using var @lock = _taskLock.UseReadLock();
		foreach (var handler in _handlers.Values)
		{
			handler?.Invoke(this, new(result));
		}
		_logger.LogTrace("[{Name}] Invoked.", name);
	}

	/// <summary>
	/// 订阅事件
	/// </summary>
	/// <param name="handler">处理器</param>
	/// <param name="priority">优先级</param>
	public void Subscribe(EventHandler<BusEventArgs<TResult>> handler, int priority = 0)
	{
		using var @lock = _handlerLock.UseWriteLock();
		if (_handlers.TryGetValue(priority, out var baseHandler))
		{
			_handlers[priority] = baseHandler + handler;
		}
		else
		{
			_handlers.Add(priority, handler);
		}

		_logger.LogTrace("[{Name}] Subscribed at priority {Priority}: {Handler}", name, priority, handler.Method.GetFullName());
	}

	/// <summary>
	/// 取消订阅事件
	/// </summary>
	/// <param name="handler">处理器</param>
	public void Unsubscribe(EventHandler<BusEventArgs<TResult>> handler)
	{
		using var @lock = _handlerLock.UseWriteLock();
		foreach (var key in _handlers.Keys.ToArray())
		{
			if (!_handlers.TryGetValue(key, out var baseHandler))
			{
				continue;
			}

			baseHandler -= handler;
			if (baseHandler == null || baseHandler.GetInvocationList().Length == 0)
			{
				_handlers.Remove(key);
			}
			else
			{
				_handlers[key] = baseHandler;
			}
		}

		_logger.LogTrace("[{Name}] Unsubscribed: {Handler}", name, handler.Method.GetFullName());
	}

	bool IEventBus.CheckSubscriptionsOnExit()
	{
		using var @lock = _handlerLock.UseReadLock();
		var result = true;
		foreach (var handler in _handlers.Values)
		{
			var invocationList = handler?.GetInvocationList();
			if (invocationList == null || invocationList.Length == 0)
			{
				continue;
			}
			foreach (var invocation in invocationList)
			{
				_logger.LogWarning("[{Name}] Event handler {Handler} is still subscribed when exiting.", name, invocation.Method.GetFullName());
				result = false;
			}
		}
		return result;
	}
}

/// <summary>
/// 事件公交车
/// </summary>
/// <typeparam name="TId">ID 类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
/// <param name="serviceProvider">服务提供者</param>
/// <param name="name">名称</param>
/// <param name="ensureUIThread">确保调用事件处理器时使用的是UI线程</param>
public class EventBus<TId, TResult>(IServiceProvider serviceProvider, string name) : IEventBus where TId : IEquatable<TId>
{
	private readonly ILogger<EventBus<TResult>> _logger = serviceProvider.GetRequiredService<ILogger<EventBus<TResult>>>();

	private readonly ConcurrentDictionary<TId, Task> _wrappedTasks = [];

	/// <summary>
	/// 加入事件队列<br/>
	/// 当车上有相同 ID 的事件时，不会加入队列，并返回 false
	/// </summary>
	/// <param name="id">事件 ID</param>
	/// <param name="taskFactory">任务</param>
	/// <returns>是否成功加入队列</returns>
	public bool Invoke(TId id, Func<Task<TResult>> taskFactory)
	{
		Task<TResult>? task = null;
		_wrappedTasks.GetOrAdd(id, _ =>
		{
			task = taskFactory();
			return task.ContinueWith(t =>
			{
				Invoke(id, t.Result);
				_wrappedTasks.Remove(id, out Task? _);
			});
		});

		if (task != null)
		{
			_logger.LogTrace("[{Name}] Task {TaskId} with event ID {EventId} enqueued.", name, task.Id, id);
		}
		else
		{
			_logger.LogTrace("[{Name}] Task with event ID {EventId} already exists.", name, id);
		}
		return task != null;
	}

	private readonly ReaderWriterLockSlim _handlerLock = new();

	private readonly SortedDictionary<int, EventHandler<BusEventArgs<TId, TResult>>> _handlers = [];

	/// <summary>
	/// 对于一些非异步事件，不需要加入队列，直接触发完成事件
	/// </summary>
	/// <param name="id">事件 ID</param>
	/// <param name="result">结果</param>
	public void Invoke(TId id, TResult result)
	{
		using var @lock = _handlerLock.UseReadLock();
		foreach (var handler in _handlers.Values)
		{
			handler?.Invoke(this, new(id, result));
		}

		_logger.LogTrace("[{Name}] Invoked event with ID {EventId}.", name, id);
	}

	/// <summary>
	/// 订阅事件
	/// </summary>
	/// <param name="handler">处理器</param>
	/// <param name="priority">优先级</param>
	public void Subscribe(EventHandler<BusEventArgs<TId, TResult>> handler, int priority = 0)
	{
		using var @lock = _handlerLock.UseWriteLock();
		if (_handlers.TryGetValue(priority, out var baseHandler))
		{
			_handlers[priority] = baseHandler + handler;
		}
		else
		{
			_handlers.Add(priority, handler);
		}

		_logger.LogTrace("[{Name}] Subscribed at priority {Priority}: {Handler}", name, priority, handler.Method.GetFullName());
	}

	/// <summary>
	/// 取消订阅事件
	/// </summary>
	/// <param name="handler">处理器</param>
	public void Unsubscribe(EventHandler<BusEventArgs<TId, TResult>> handler)
	{
		using var @lock = _handlerLock.UseWriteLock();
		foreach (var key in _handlers.Keys.ToArray())
		{
			if (!_handlers.TryGetValue(key, out var baseHandler))
			{
				continue;
			}

			baseHandler -= handler;
			if (baseHandler == null || baseHandler.GetInvocationList().Length == 0)
			{
				_handlers.Remove(key);
			}
			else
			{
				_handlers[key] = baseHandler;
			}
		}

		_logger.LogTrace("[{Name}] Unsubscribed: {Handler}", name, handler.Method.GetFullName());
	}

	bool IEventBus.CheckSubscriptionsOnExit()
	{
		using var @lock = _handlerLock.UseReadLock();
		var result = true;
		foreach (var handler in _handlers.Values)
		{
			var invocationList = handler?.GetInvocationList();
			if (invocationList == null || invocationList.Length == 0)
			{
				continue;
			}
			foreach (var invocation in invocationList)
			{
				_logger.LogWarning("[{Name}] Event handler {Handler} is still subscribed when exiting.", name, invocation.Method.GetFullName());
				result = false;
			}
		}
		return result;
	}
}
