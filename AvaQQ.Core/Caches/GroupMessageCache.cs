using AvaQQ.Core.Events;
using AvaQQ.Core.Messages;
using AvaQQ.Core.Utils;

namespace AvaQQ.Core.Caches;

internal class GroupMessageCache : IGroupMessageCache
{
	private readonly EventStation _events;

	public GroupMessageCache(
		EventStation events
		)
	{
		_events = events;

		_events.OnGroupMessage.Subscribe(OnGroupMessage);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_events.OnGroupMessage.Unsubscribe(OnGroupMessage);

			if (disposing)
			{
				_previewLock.Dispose();
			}

			disposedValue = true;
		}
	}

	~GroupMessageCache()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion

	public string GetLatestMessageTime(ulong uin)
	{
		return string.Empty;
	}

	private readonly ReaderWriterLockSlim _previewLock = new();

	private readonly Dictionary<ulong, string> _previewCaches = [];

	public string GetLatestMessagePreview(ulong uin)
	{
		using var _ = _previewLock.UseReadLock();
		if (_previewCaches.TryGetValue(uin, out var preview))
		{
			return preview;
		}
		return string.Empty;
	}

	private void OnGroupMessage(object? sender, BusEventArgs<Message> e)
	{
		using var _ = _previewLock.UseWriteLock();
		_previewCaches[e.Result.GroupUin!.Value] = e.Result.Preview;
	}
}
