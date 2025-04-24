using AvaQQ.Core.Caches;
using AvaQQ.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace AvaQQ.Core.Databases;

internal class SqliteDatabase : Database
{
	private readonly EventStation _events;

	public SqliteDatabase(EventStation events)
	{
		_events = events;

		_events.CachedGetAllFriends.OnDone += OnCachedGetAllFriends;
		_events.CachedGetAllJoinedGroups.OnDone += OnCachedGetAllJoinedGroups;
	}

	private UserSqliteContext? _context;

	public UserSqliteContext Context => _context ?? throw new InvalidOperationException("Database not initialized.");

	public override void Initialize(ulong uin)
	{
		if (_context != null)
		{
			_context.Dispose();
			_context = null;
		}

		_context = new UserSqliteContext(uin);
		_context.Database.EnsureCreated();
		Context.SaveChanges();
	}

	public override RecordedGroupInfo[] GetAllRecordedGroups()
		=> [.. Context.Groups];

	public override RecordedUserInfo[] GetAllRecordedUsers()
		=> [.. Context.Users];

	#region 事件处理

	private void OnCachedGetAllJoinedGroups(object? sender, BusEventArgs<CommonEventId, CachedGroupInfo[]> e)
	{
		Context.Groups
			.UpsertRange(e.Result.Select<CachedGroupInfo, RecordedGroupInfo>(v => v))
			.Run();
		Context.SaveChanges();

		foreach (var info in e.Result)
		{
			info.HasLocalData = true;
		}
	}

	private void OnCachedGetAllFriends(object? sender, BusEventArgs<CommonEventId, CachedUserInfo[]> e)
	{
		Context.Users
			.UpsertRange(e.Result.Select<CachedUserInfo, RecordedUserInfo>(v => v))
			.Run();
		Context.SaveChanges();

		foreach (var info in e.Result)
		{
			info.HasLocalData = true;
		}
	}

	#endregion

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_context?.Dispose();
			}

			disposedValue = true;
		}
	}

	~SqliteDatabase()
	{
		_events.CachedGetAllJoinedGroups.OnDone -= OnCachedGetAllJoinedGroups;

		Dispose(disposing: false);
	}

	public override void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion
}
