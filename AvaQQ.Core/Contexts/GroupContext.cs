using AvaQQ.Core.Events;
using Microsoft.Extensions.Logging;

namespace AvaQQ.Core.Contexts;

internal partial class GroupContext : IGroupContext
{
	private readonly ILogger<GroupContext> _logger;

	private readonly EventStation _events;

	public GroupContext(ILogger<GroupContext> logger, IAvatarCacheProvider avatarCacheProvider, EventStation events)
	{
		_logger = logger;
		_events = events;

		_avatarCache = avatarCacheProvider.Get("group");
		InitializeAvatar();
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			FinalizeAvatar();

			if (disposing)
			{
			}

			disposedValue = true;
		}
	}

	~GroupContext()
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
