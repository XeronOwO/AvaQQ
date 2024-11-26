using AvaQQ.SDK.Databases;
using LiteDB;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace AvaQQ.Databases;

internal class GroupMessageLiteDB : GroupMessageDatabase
{
	private readonly ConcurrentDictionary<long, LiteDatabase> _databases = [];

	private LiteDatabase CreateDatabase(long groupUin)
		=> new(Path.Combine(BaseDirectory, $"group-{groupUin}.db"));

	private LiteDatabase GetOrCreateDatabase(long groupUin)
		=> _databases.GetOrAdd(groupUin, CreateDatabase);

	public override void Insert(GroupMessageEntry entry)
	{
		GetOrCreateDatabase(entry.GroupUin)
			.GetCollection<GroupMessageEntry>("messages").Insert(entry);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
			}

			foreach (var (_, database) in _databases)
			{
				database.Dispose();
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
