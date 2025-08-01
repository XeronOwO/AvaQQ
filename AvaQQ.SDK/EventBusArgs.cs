namespace AvaQQ.SDK;

public class EventBusArgsBase : EventArgs
{
	public bool IsHandled { get; set; } = false;
}

public class EventBusArgs(object? value) : EventBusArgsBase
{
	public object? Result { get; } = value;
}

public class EventBusArgs<TResult>(TResult value) : EventBusArgs(value)
{
	public new TResult? Result => (TResult?)base.Result;
}

public class KeyedEventBusArgs(object key, object? value) : EventBusArgsBase
{
	public object Key { get; } = key;

	public object? Result { get; } = value;
}

public class KeyedEventBusArgs<TId, TResult>(TId key, TResult value) : KeyedEventBusArgs(key, value)
	where TId : notnull
{
	public new TId Key => (TId)base.Key;

	public new TResult? Result => (TResult?)base.Result;
}
