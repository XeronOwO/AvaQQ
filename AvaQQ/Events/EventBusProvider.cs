using AvaQQ.Resources;
using AvaQQ.SDK;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AvaQQ.Events;

internal class EventBusProvider(ILogger<EventBusProvider> logger) : IEventBusProvider, IDisposable
{
	private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, IEventBusBase>> _buses = [];

	public bool TryAdd(string category, string name, IEventBusBase bus)
	{
		var buses = _buses.GetOrAdd(category, _ => []);

		if (!buses.TryAdd(name, bus))
		{
			logger.LogWarning("Event bus with name '{Name}' and category '{Category}' is already registered", name, category);
			return false;
		}

		logger.LogInformation("Registered event bus with name '{Name}' and category '{Category}'", name, category);
		return true;
	}

	public bool TryRemove(string category, string name, [NotNullWhen(true)] out IEventBusBase? bus)
	{
		if (_buses.TryGetValue(category, out var buses) && buses.TryRemove(name, out bus))
		{
			logger.LogInformation("Removed event bus with name '{Name}' and category '{Category}'", name, category);
			return true;
		}

		bus = null;
		logger.LogWarning("Event bus with name '{Name}' and category '{Category}' not found", name, category);
		return false;
	}

	public bool TryGet(string category, string name, [NotNullWhen(true)] out IEventBusBase? bus)
	{
		if (_buses.TryGetValue(category, out var buses) && buses.TryGetValue(name, out bus))
		{
			return true;
		}

		bus = null;
		return false;
	}

	public bool TryGet<TResult>(string category, string name, [NotNullWhen(true)] out IEventBus<TResult>? bus)
	{
		if (TryGet(category, name, out var busBase) &&
			busBase is IEventBus<TResult> typedBus)
		{
			bus = typedBus;
			return true;
		}

		bus = null;
		return false;
	}

	public bool TryGet<TKey, TResult>(string category, string name, [NotNullWhen(true)] out IKeyedEventBus<TKey, TResult>? bus)
		where TKey : notnull
	{
		if (TryGet(category, name, out var busBase) &&
			busBase is IKeyedEventBus<TKey, TResult> typedBus)
		{
			bus = typedBus;
			return true;
		}

		bus = null;
		return false;
	}

	public IEventBusBase Get(string category, string name)
	{
		if (!TryGet(category, name, out var bus))
		{
			throw new KeyNotFoundException(string.Format(SR.ExceptionEventBusNotFound, name, category));
		}

		return bus;
	}

	public IEventBus<TResult> Get<TResult>(string category, string name)
	{
		var busBase = Get(category, name);
		if (busBase is not IEventBus<TResult> typedBus)
		{
			throw new InvalidCastException(string.Format(SR.ExceptionEventBusCast, name, category, typeof(IEventBus<TResult>).FullName, busBase.GetType().FullName));
		}

		return typedBus;
	}

	public IKeyedEventBus<TKey, TResult> Get<TKey, TResult>(string category, string name)
		where TKey : notnull
	{
		var busBase = Get(category, name);
		if (busBase is not IKeyedEventBus<TKey, TResult> typedBus)
		{
			throw new InvalidCastException(string.Format(SR.ExceptionEventBusCast, name, category, typeof(IKeyedEventBus<TKey, TResult>).FullName, busBase.GetType().FullName));
		}
		return typedBus;
	}

	private void CheckSubscriptionsOnExit()
	{
		var success = true;
		foreach (var (_, buses) in _buses)
		{
			foreach (var (_, bus) in buses)
			{
				success = bus.CheckSubscriptionsOnExit() && success;
			}
		}

		if (!success)
		{
			logger.LogWarning("Some event buses have subscriptions that are still active when exiting the application.");
			Debug.Assert(false, "Some event buses have subscriptions that are still active when exiting the application. This should not happen.");
		}
		else
		{
			logger.LogInformation("All event buses have no active subscriptions on exit");
		}
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				CheckSubscriptionsOnExit();
			}

			disposedValue = true;
		}
	}

	~EventBusProvider()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion
}
