using LiteDB;
using System.Collections.Concurrent;

namespace AvaQQ.Core.Databases;

internal class GroupMessageLiteDB : GroupMessageDatabase
{
	private readonly ConcurrentDictionary<ulong, LiteDatabase> _databases = [];

	private LiteDatabase CreateDatabase(ulong groupUin)
		=> new(Path.Combine(BaseDirectory, $"group-{groupUin}.db"));

	private LiteDatabase GetOrCreateDatabase(ulong groupUin)
		=> _databases.GetOrAdd(groupUin, CreateDatabase);

	public override void Insert(ulong groupUin, GroupMessageEntry entry)
		=> GetOrCreateDatabase(groupUin)
		.GetCollection<GroupMessageEntry>("messages")
		.Insert(entry);

	public override GroupMessageEntry? Latest(ulong groupUin)
		=> GetOrCreateDatabase(groupUin)
		.GetCollection<GroupMessageEntry>("messages")
		.Query()
		.OrderByDescending(x => x.Time)
		.Limit(1)
		.FirstOrDefault();

	public override void Sync(ulong groupUin, IEnumerable<GroupMessageEntry> entries)
	{
		var collection = GetOrCreateDatabase(groupUin)
			.GetCollection<GroupMessageEntry>("messages");

		collection.InsertBulk(
			entries.Where(entry => !collection.Exists(
				record => record.MessageId == entry.MessageId && record.Time == entry.Time
			))
		);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				foreach (var (_, database) in _databases)
				{
					database.Dispose();
				}
			}

			disposedValue = true;
		}
	}

	~GroupMessageLiteDB()
	{
		Dispose(disposing: false);
	}

	public override void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion
}
