using AvaQQ.SDK.Resources;

namespace AvaQQ.SDK;

public interface IEventBusBase
{
	bool IsKeyed { get; }

	Type ResultType { get; }

	bool CheckSubscriptionsOnExit();
}

public interface IEventBus : IEventBusBase
{
	bool IEventBusBase.IsKeyed => false;

	bool Invoke(Func<Task<object>> taskFactory);

	void Invoke(object result);

	void Subscribe(EventHandler<EventBusArgs> handler, int priority = 0);

	void Unsubscribe(EventHandler<EventBusArgs> handler);
}

public interface IEventBus<TResult> : IEventBus
{
	Type IEventBusBase.ResultType => typeof(TResult);

	bool IEventBus.Invoke(Func<Task<object>> taskFactory)
		=> Invoke(async () =>
		{
			var result = await taskFactory();
			if (result is not TResult typedResult)
			{
				throw new ArgumentException(string.Format(SR.ExceptionEventBusResultType, typeof(TResult).FullName, result.GetType().FullName));
			}

			return typedResult;
		});

	bool Invoke(Func<Task<TResult>> taskFactory);

	void IEventBus.Invoke(object result)
	{
		if (result is TResult typedResult)
		{
			Invoke(typedResult);
		}
		else
		{
			throw new ArgumentException(string.Format(SR.ExceptionEventBusResultType, typeof(TResult).FullName, result.GetType().FullName));
		}
	}

	void Invoke(TResult result);

	void IEventBus.Subscribe(EventHandler<EventBusArgs> handler, int priority)
		=> Subscribe((sender, args) => handler(sender, args), priority);

	void Subscribe(EventHandler<EventBusArgs<TResult>> handler, int priority = 0);

	void IEventBus.Unsubscribe(EventHandler<EventBusArgs> handler)
		=> Unsubscribe((sender, args) => handler(sender, args));

	void Unsubscribe(EventHandler<EventBusArgs<TResult>> handler);
}

public interface IKeyedEventBus : IEventBusBase
{
	Type KeyType { get; }

	bool IEventBusBase.IsKeyed => true;

	bool Invoke(object key, Func<Task<object>> taskFactory);

	void Invoke(object key, object result);

	void Subscribe(EventHandler<EventBusArgs> handler, int priority = 0);

	void Unsubscribe(EventHandler<EventBusArgs> handler);
}

public interface IKeyedEventBus<TKey, TResult> : IKeyedEventBus
	where TKey : notnull
{
	Type IKeyedEventBus.KeyType => typeof(TKey);

	Type IEventBusBase.ResultType => typeof(TResult);

	bool IKeyedEventBus.Invoke(object key, Func<Task<object>> taskFactory)
	{
		if (key is not TKey typedId)
		{
			throw new ArgumentException(string.Format(SR.ExceptionEventBusKeyType, typeof(TKey).FullName, key.GetType().FullName));
		}

		return Invoke(typedId, async () =>
		{
			var result = await taskFactory();
			if (result is not TResult typedResult)
			{
				throw new ArgumentException(string.Format(SR.ExceptionEventBusResultType, typeof(TResult).FullName, result.GetType().FullName));
			}

			return typedResult;
		});
	}

	bool Invoke(TKey key, Func<Task<TResult>> taskFactory);

	void IKeyedEventBus.Invoke(object key, object result)
	{
		if (key is not TKey typedKey)
		{
			throw new ArgumentException(string.Format(SR.ExceptionEventBusKeyType, typeof(TKey).FullName, key.GetType().FullName));
		}
		if (result is not TResult typedResult)
		{
			throw new ArgumentException(string.Format(SR.ExceptionEventBusResultType, typeof(TResult).FullName, result.GetType().FullName));
		}

		Invoke(typedKey, typedResult);
	}

	void Invoke(TKey key, TResult result);

	void IKeyedEventBus.Subscribe(EventHandler<EventBusArgs> handler, int priority)
		=> Subscribe((sender, args) => handler(sender, args), priority);

	void Subscribe(EventHandler<KeyedEventBusArgs<TKey, TResult>> handler, int priority = 0);

	void IKeyedEventBus.Unsubscribe(EventHandler<EventBusArgs> handler)
		=> Unsubscribe((sender, args) => handler(sender, args));

	void Unsubscribe(EventHandler<KeyedEventBusArgs<TKey, TResult>> handler);
}
