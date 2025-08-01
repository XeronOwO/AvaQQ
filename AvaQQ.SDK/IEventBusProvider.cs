using System.Diagnostics.CodeAnalysis;

namespace AvaQQ.SDK;

public interface IEventBusProvider
{
	IEventBusBase Get(string category, string name);

	IEventBus<TResult> Get<TResult>(string category, string name);

	IKeyedEventBus<TKey, TResult> Get<TKey, TResult>(string category, string name) where TKey : notnull;

	bool TryAdd(string category, string name, IEventBusBase bus);

	bool TryGet(string category, string name, [NotNullWhen(true)] out IEventBusBase? bus);

	bool TryGet<TResult>(string category, string name, [NotNullWhen(true)] out IEventBus<TResult>? bus);

	bool TryGet<TKey, TResult>(string category, string name, [NotNullWhen(true)] out IKeyedEventBus<TKey, TResult>? bus) where TKey : notnull;

	bool TryRemove(string category, string name, [NotNullWhen(true)] out IEventBusBase? bus);
}
