using AvaQQ.Events;
using AvaQQ.Exceptions;
using AvaQQ.SDK;
using AvaQQ.SDK.Entities;
using Microsoft.EntityFrameworkCore;

namespace AvaQQ.Databases;

internal class SqliteDatabase : IDatabase
{
	private readonly AvaQQEvents _events;

	public SqliteDatabase(AvaQQEvents events)
	{
		_events = events;
	}

	private DatabaseContext? _context;

	public DatabaseContext Context => _context ?? throw new NotInitializedException(nameof(Context));

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

	public async Task<IUserInfo?> GetUserAsync(ulong uin, CancellationToken token)
		=> await Context.Users.Where(u => u.Uin == uin).FirstOrDefaultAsync(token);

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
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
}
