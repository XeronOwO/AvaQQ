using AvaQQ.Core.Events;
using Microsoft.Extensions.Logging;

namespace AvaQQ.Core.Contexts;

internal partial class UserContext : IUserContext
{
	private readonly ILogger<UserContext> _logger;

	private readonly EventStation _events;

	public UserContext(
		ILogger<UserContext> logger,
		IAvatarCacheProvider avatarCacheProvider,
		EventStation events)
	{
		_logger = logger;
		_events = events;
		_avatarCache = avatarCacheProvider.Get("user");
		_userCache = userCache;

		InitializeAvatar();
		InitializeInfo();
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			FinalizeAvatar();
			FinalizeInfo();

			if (disposing)
			{
			}

			disposedValue = true;
		}
	}

	~UserContext()
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
