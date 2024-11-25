using AvaQQ.SDK.Databases;
using LiteDB;
using System;
using System.IO;

namespace AvaQQ.Databases;

internal class GroupMessageLiteDB : GroupMessageDatabase
{
	private static readonly string _databasePath = Path.Combine(BaseDirectory, "group_message.db");

	private readonly LiteDatabase _database = new(_databasePath);

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
			}

			_database.Dispose();
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
