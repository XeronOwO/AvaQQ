using AvaQQ.SDK;
using AvaQQ.SDK.Utils;
using AvaQQ.Utils;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace AvaQQ.Events;

public class EventBus<TResult>(ILogger<EventBus<TResult>> logger, string name) : IEventBus<TResult>
{
	private readonly ReaderWriterLockSlim _taskLock = new();

	private Task? _wrappedTask;

	public bool Invoke(Func<Task<TResult>> taskFactory)
	{
		using var lock1 = _taskLock.UseUpgradeableReadLock();
		if (_wrappedTask != null)
		{
			logger.LogTrace("[{Name}] Task already exists", name);
			return false;
		}

		using var lock2 = _taskLock.UseWriteLock();
		if (_wrappedTask != null)
		{
			logger.LogTrace("[{Name}] Task already exists", name);
			return false;
		}

		var task = taskFactory();
		_wrappedTask = task.ContinueWith(t =>
		{
			_wrappedTask = null;
			Invoke(t.Result);
			using var lock3 = _taskLock.UseWriteLock();
			logger.LogTrace("[{Name}] Task {TaskId} done", name, task.Id);
		});
		logger.LogTrace("[{Name}] Task {TaskId} enqueued", name, task.Id);
		return true;
	}

	private readonly ReaderWriterLockSlim _handlerLock = new();

	private readonly SortedDictionary<int, EventHandler<EventBusArgs<TResult>>> _handlers = [];

	public void Invoke(TResult result)
	{
		logger.LogTrace("[{Name}] Begin invoke", name);
		using var @lock = _taskLock.UseReadLock();
		var args = new EventBusArgs<TResult>(result);
		foreach (var handler in _handlers.Values)
		{
			if (handler is null)
			{
				continue;
			}

			try
			{
				logger.LogTrace("[{Name}] Invoking handler {Handler}", name, handler.Method.GetFullName());
				handler.Invoke(this, args);
				if (args.IsHandled)
				{
					logger.LogTrace($"[{{Name}}] {nameof(args.IsHandled)} == true. Skipping", name);
					break;
				}
			}
			catch (Exception e)
			{
				logger.LogError(e, "[{Name}] Exception occurred while invoking event", name);
			}
		}
		logger.LogTrace("[{Name}] End invoke", name);
	}

	public void Subscribe(EventHandler<EventBusArgs<TResult>> handler, int priority = 0)
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

		logger.LogTrace("[{Name}] Subscribed at priority {Priority}: {Handler}", name, priority, handler.Method.GetFullName());
	}

	public void Unsubscribe(EventHandler<EventBusArgs<TResult>> handler)
	{
		using var @lock = _handlerLock.UseWriteLock();
		foreach (var key in _handlers.Keys.ToArray())
		{
			if (!_handlers.TryGetValue(key, out var baseHandler))
			{
				continue;
			}

			baseHandler -= handler;
			if (baseHandler == null)
			{
				_handlers.Remove(key);
			}
			else
			{
				_handlers[key] = baseHandler;
			}
		}

		logger.LogTrace("[{Name}] Unsubscribed: {Handler}", name, handler.Method.GetFullName());
	}

	public bool CheckSubscriptionsOnExit()
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
				logger.LogWarning("[{Name}] Event handler {Handler} is still subscribed when exiting", name, invocation.Method.GetFullName());
				result = false;
			}
		}
		return result;
	}
}

public class EventBus<TKey, TResult>(ILogger<EventBus<TKey, TResult>> logger, string name) : IKeyedEventBus<TKey, TResult> where TKey : notnull
{
	private readonly ConcurrentDictionary<TKey, Task> _wrappedTasks = [];

	public bool Invoke(TKey key, Func<Task<TResult>> taskFactory)
	{
		Task<TResult>? task = null;
		_wrappedTasks.GetOrAdd(key, _ =>
		{
			task = taskFactory();
			return task.ContinueWith(t =>
			{
				_wrappedTasks.Remove(key, out Task? _);
				Invoke(key, t.Result);
			});
		});

		if (task != null)
		{
			logger.LogTrace("[{Name}] Task {TaskId} with event key {EventKey} enqueued", name, task.Id, key);
		}
		else
		{
			logger.LogTrace("[{Name}] Task with event key {EventKey} already exists", name, key);
		}
		return task != null;
	}

	private readonly ReaderWriterLockSlim _handlerLock = new();

	private readonly SortedDictionary<int, EventHandler<KeyedEventBusArgs<TKey, TResult>>> _handlers = [];

	public void Invoke(TKey key, TResult result)
	{
		logger.LogTrace("[{Name}] Begin invoke event with key {EventKey}", name, key);
		using var @lock = _handlerLock.UseReadLock();
		var args = new KeyedEventBusArgs<TKey, TResult>(key, result);
		foreach (var handler in _handlers.Values)
		{
			if (handler is null)
			{
				continue;
			}

			try
			{
				logger.LogTrace("[{Name}] Invoking handler {Handler} with key {EventKey}", name, handler.Method.GetFullName(), key);
				handler.Invoke(this, args);
				if (args.IsHandled)
				{
					logger.LogTrace($"[{{Name}}] {nameof(args.IsHandled)} == true. Skipping", name);
					break;
				}
			}
			catch (Exception e)
			{
				logger.LogError(e, "[{Name}] Exception occurred while invoking event with key {EventKey}", name, key);
			}
		}
		logger.LogTrace("[{Name}] End invoke event with key {EventKey}", name, key);
	}

	public void Subscribe(EventHandler<KeyedEventBusArgs<TKey, TResult>> handler, int priority = 0)
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

		logger.LogTrace("[{Name}] Subscribed at priority {Priority}: {Handler}", name, priority, handler.Method.GetFullName());
	}

	public void Unsubscribe(EventHandler<KeyedEventBusArgs<TKey, TResult>> handler)
	{
		using var @lock = _handlerLock.UseWriteLock();
		foreach (var key in _handlers.Keys.ToArray())
		{
			if (!_handlers.TryGetValue(key, out var baseHandler))
			{
				continue;
			}

			baseHandler -= handler;
			if (baseHandler == null)
			{
				_handlers.Remove(key);
			}
			else
			{
				_handlers[key] = baseHandler;
			}
		}

		logger.LogTrace("[{Name}] Unsubscribed: {Handler}", name, handler.Method.GetFullName());
	}

	public bool CheckSubscriptionsOnExit()
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
				logger.LogWarning("[{Name}] Event handler {Handler} is still subscribed when exiting", name, invocation.Method.GetFullName());
				result = false;
			}
		}
		return result;
	}
}
