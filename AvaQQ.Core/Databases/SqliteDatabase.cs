using AvaQQ.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace AvaQQ.Core.Databases;

internal class SqliteDatabase : IDatabase
{
	private readonly EventStation _events;

	public SqliteDatabase(EventStation events)
	{
		_events = events;

		_events.OnNewUserCached.Subscribe(OnNewUserCached);
		_events.OnUserNicknameChanged.Subscribe(OnUserNicknameChanged);
		_events.OnUserRemarkChanged.Subscribe(OnUserRemarkChanged);
		_events.OnNewGroupCached.Subscribe(OnNewGroupCached);
		_events.OnGroupNameChanged.Subscribe(OnGroupNameChanged);
		_events.OnGroupRemarkChanged.Subscribe(OnGroupRemarkChanged);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_events.OnNewUserCached.Unsubscribe(OnNewUserCached);
			_events.OnUserNicknameChanged.Unsubscribe(OnUserNicknameChanged);
			_events.OnUserRemarkChanged.Unsubscribe(OnUserRemarkChanged);
			_events.OnNewGroupCached.Unsubscribe(OnNewGroupCached);
			_events.OnGroupNameChanged.Unsubscribe(OnGroupNameChanged);
			_events.OnGroupRemarkChanged.Unsubscribe(OnGroupRemarkChanged);

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

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion

	private DatabaseContext? _context;

	public DatabaseContext Context => _context ?? throw new InvalidOperationException("Database not initialized.");

	public void Initialize(ulong uin)
	{
		if (_context != null)
		{
			_context.Dispose();
			_context = null;
		}

		_context = new DatabaseContext(uin);
		_context.Database.EnsureCreated();
		Context.SaveChanges();
	}

	public Task<RecordedGroupInfo[]> GetAllRecordedGroupsAsync(CancellationToken token = default)
		=> Context.Groups.ToArrayAsync(token);

	public Task<RecordedUserInfo[]> GetAllRecordedUsersAsync(CancellationToken token = default)
		=> Context.Users.ToArrayAsync(token);

	#region 事件处理

	private async void OnNewUserCached(object? sender, BusEventArgs<CachedUserInfo> e)
	{
		await Context.Users
			.Upsert(e.Result)
			.RunAsync();
		await Context.SaveChangesAsync();
	}

	private async void OnUserNicknameChanged(object? sender, BusEventArgs<UserNicknameChangedInfo> e)
	{
		await Context.Users
			.Upsert(e.Result.Cache)
			.RunAsync();
		await Context.SaveChangesAsync();
	}

	private async void OnUserRemarkChanged(object? sender, BusEventArgs<UserRemarkChangedInfo> e)
	{
		await Context.Users
			.Upsert(e.Result.Cache)
			.RunAsync();
		await Context.SaveChangesAsync();
	}

	private async void OnNewGroupCached(object? sender, BusEventArgs<CachedGroupInfo> e)
	{
		await Context.Groups
			.Upsert(e.Result)
			.RunAsync();
		await Context.SaveChangesAsync();
	}

	private async void OnGroupNameChanged(object? sender, BusEventArgs<GroupNameChangedInfo> e)
	{
		await Context.Groups
			.Upsert(e.Result.Cache)
			.RunAsync();
		await Context.SaveChangesAsync();
	}

	private async void OnGroupRemarkChanged(object? sender, BusEventArgs<GroupRemarkChangedInfo> e)
	{
		await Context.Groups
			.Upsert(e.Result.Cache)
			.RunAsync();
		await Context.SaveChangesAsync();
	}

	#endregion
}
