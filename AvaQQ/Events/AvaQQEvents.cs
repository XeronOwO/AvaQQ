using AvaQQ.SDK;
using AvaQQ.SDK.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace AvaQQ.Events;

public class AvaQQEvents
{
	public AvaQQEvents(IEventBusProvider events, IServiceProvider serviceProvider)
	{
		OnTrayIconClicked = Event<EmptyEventResult>(nameof(OnTrayIconClicked));
		OnLoggedIn = Event<IAdapter>(nameof(OnLoggedIn));
		OnAvatarFetched = KeyedEvent<AvatarId, byte[]?>(nameof(OnAvatarFetched));
		OnAvatarChanged = KeyedEvent<AvatarId, AvatarChangedInfo>(nameof(OnAvatarChanged));
		OnAdaptedUserInfoFetched = KeyedEvent<ulong, AdaptedUserInfo?>(nameof(OnAdaptedUserInfoFetched));
		OnUserInfoFetched = KeyedEvent<ulong, IUserInfo?>(nameof(OnUserInfoFetched));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		EventBus<TResult> Event<TResult>(string name)
		{
			var bus = new EventBus<TResult>(
				serviceProvider.GetRequiredService<ILogger<EventBus<TResult>>>(),
				name
			);
			events.TryAdd(nameof(AvaQQ), name, bus);
			return bus;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		EventBus<TKey, TResult> KeyedEvent<TKey, TResult>(string name)
			where TKey : notnull
		{
			var bus = new EventBus<TKey, TResult>(
				serviceProvider.GetRequiredService<ILogger<EventBus<TKey, TResult>>>(),
				name
			);
			events.TryAdd(nameof(AvaQQ), name, bus);
			return bus;
		}
	}

	public EventBus<EmptyEventResult> OnTrayIconClicked { get; }

	public EventBus<IAdapter> OnLoggedIn { get; }

	public EventBus<AvatarId, byte[]?> OnAvatarFetched { get; }

	public EventBus<AvatarId, AvatarChangedInfo> OnAvatarChanged { get; }

	public EventBus<ulong, AdaptedUserInfo?> OnAdaptedUserInfoFetched { get; }

	public EventBus<ulong, IUserInfo?> OnUserInfoFetched { get; }
}
