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

		_events.CachedGetAllFriends.Subscribe(OnCachedGetAllFriends);
		_events.CachedGetAllJoinedGroups.Subscribe(OnCachedGetAllJoinedGroups);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_events.CachedGetAllFriends.Subscribe(OnCachedGetAllFriends);
			_events.CachedGetAllJoinedGroups.Subscribe(OnCachedGetAllJoinedGroups);

			if (disposing)
			{
				_context?.Dispose();
			}

			disposedValue = true;
		}
	}

	~SqliteDatabase()
	{
		Dispose(disposing: false);
	}

	public override void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion

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

	public override Task<RecordedGroupInfo[]> GetAllRecordedGroupsAsync(CancellationToken token = default)
		=> Context.Groups.ToArrayAsync(token);

	public override Task<RecordedUserInfo[]> GetAllRecordedUsersAsync(CancellationToken token = default)
		=> Context.Users.ToArrayAsync(token);

	#region 事件处理

	private async void OnCachedGetAllJoinedGroups(object? sender, BusEventArgs<CommonEventId, CachedGroupInfo[]> e)
	{
		foreach (var info in e.Result)
		{
			info.HasLocalData = true;
		}

		await Context.Groups
			.UpsertRange(e.Result.Select<CachedGroupInfo, RecordedGroupInfo>(v => v))
			.RunAsync();
		await Context.SaveChangesAsync();
	}

	private async void OnCachedGetAllFriends(object? sender, BusEventArgs<CommonEventId, CachedUserInfo[]> e)
	{
		foreach (var info in e.Result)
		{
			info.HasLocalData = true;
		}

		await Context.Users
			.UpsertRange(e.Result.Select<CachedUserInfo, RecordedUserInfo>(v => v))
			.RunAsync();
		await Context.SaveChangesAsync();
	}

	#endregion
}
