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
			.Find(Query.All(nameof(GroupMessageEntry.Time), Query.Descending), limit: 1)
			.FirstOrDefault();

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
