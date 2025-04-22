using AvaQQ.Core.Caches;
using AvaQQ.Core.Events;
using Microsoft.Data.Sqlite;

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

	private SqliteConnection? _connection;

	public override void Initialize(ulong uin)
	{
		if (_connection != null)
		{
			_connection.Close();
			_connection.Dispose();
			_connection = null;
		}

		_connection = new SqliteConnection(new SqliteConnectionStringBuilder()
		{
			DataSource = Path.Combine(BaseDirectory, $"user-{uin}.db"),
		}.ToString());
		_connection.Open();

		InitVersion();
		InitTables();
		InitCommands(_connection);
	}

	private const int Version = 1;

	private void InitVersion()
	{
		if (_connection == null)
		{
			throw new InvalidOperationException("Database not initialized.");
		}

		using var command = _connection.CreateCommand();
		command.CommandText = "PRAGMA user_version;";
		var version = (long)command.ExecuteScalar()!;
		if (version == 0)
		{
			command.CommandText = $"PRAGMA user_version = {Version};";
			command.ExecuteNonQuery();
		}
		else
		{
			if (version != Version)
			{
				throw new InvalidOperationException($"Database version mismatch. Expected {Version}, but got {version}.");
			}
		}
	}

	private void InitTables()
	{
		if (_connection == null)
		{
			throw new InvalidOperationException("Database not initialized.");
		}

		using var command = _connection.CreateCommand();
		command.CommandText = @"
			CREATE TABLE IF NOT EXISTS `user` (
				`uin` BIGINT UNSIGNED PRIMARY KEY NOT NULL,
				`name` VARCHAR(128) NOT NULL,
				`remark` VARCHAR(128)
			);
			CREATE INDEX IF NOT EXISTS `idx_user_name` ON `user` (`name`);
			CREATE INDEX IF NOT EXISTS `idx_user_remark` ON `user` (`remark`);

			CREATE TABLE IF NOT EXISTS `group` (
				`uin` BIGINT UNSIGNED PRIMARY KEY NOT NULL,
				`name` VARCHAR(128) NOT NULL,
				`remark` VARCHAR(128)
			);
			CREATE INDEX IF NOT EXISTS `idx_group_name` ON `group` (`name`);
			CREATE INDEX IF NOT EXISTS `idx_group_remark` ON `group` (`remark`);

			CREATE TABLE IF NOT EXISTS `group_member` (
				`id` INTEGER PRIMARY KEY AUTOINCREMENT,
				`group_uin` BIGINT UNSIGNED NOT NULL,
				`uin` BIGINT UNSIGNED NOT NULL,
				`perm` TINYINT UNSIGNED NOT NULL,
				`lvl` TINYINT UNSIGNED NOT NULL,
				`card` VARCHAR(128) NOT NULL,
				`name` VARCHAR(128) NOT NULL,
				`title` VARCHAR(128) NOT NULL,
				`join_time` REAL NOT NULL,
				`last_msg_time` REAL NOT NULL
			);
			CREATE UNIQUE INDEX IF NOT EXISTS `idx_group_member_group_uin` ON `group_member` (`group_uin`);
			CREATE UNIQUE INDEX IF NOT EXISTS `idx_group_member_uin` ON `group_member` (`uin`);

			CREATE TABLE IF NOT EXISTS `txt_seg` (
				`id` INTEGER PRIMARY KEY AUTOINCREMENT,
				`type` TINYINT UNSIGNED NOT NULL,
				`group_uin` BIGINT UNSIGNED NOT NULL,
				`sender_uin` BIGINT UNSIGNED NOT NULL,
				`seq` BIGINT UNSIGNED NOT NULL,
				`time` REAL NOT NULL,
				`idx` SMALLINT UNSIGNED NOT NULL,
				`txt` TEXT NOT NULL
			);
			CREATE INDEX IF NOT EXISTS `idx_txt_seg_type` ON `txt_seg` (`type`);
			CREATE INDEX IF NOT EXISTS `idx_txt_seg_group_uin` ON `txt_seg` (`group_uin`);
			CREATE INDEX IF NOT EXISTS `idx_txt_seg_sender_uin` ON `txt_seg` (`sender_uin`);
			CREATE INDEX IF NOT EXISTS `idx_txt_seg_time` ON `txt_seg` (`time`);
			CREATE VIRTUAL TABLE IF NOT EXISTS `vt_txt_seg_txt` USING FTS5(`txt`);

			CREATE TABLE IF NOT EXISTS `pic_seg` (
				`id` INTEGER PRIMARY KEY AUTOINCREMENT,
				`type` TINYINT UNSIGNED NOT NULL,
				`group_uin` BIGINT UNSIGNED NOT NULL,
				`sender_uin` BIGINT UNSIGNED NOT NULL,
				`seq` BIGINT UNSIGNED NOT NULL,
				`time` REAL NOT NULL,
				`idx` SMALLINT UNSIGNED NOT NULL,
				`file_id` VARCHAR NOT NULL,
				`width` INTEGER NOT NULL,
				`height` INTEGER NOT NULL,
				`sha1` VARCHAR NOT NULL,
				`summary` VARCHAR NOT NULL
			);
			CREATE INDEX IF NOT EXISTS `idx_pic_seg_type` ON `pic_seg` (`type`);
			CREATE INDEX IF NOT EXISTS `idx_pic_seg_group_uin` ON `pic_seg` (`group_uin`);
			CREATE INDEX IF NOT EXISTS `idx_pic_seg_sender_uin` ON `pic_seg` (`sender_uin`);
			CREATE INDEX IF NOT EXISTS `idx_pic_seg_time` ON `pic_seg` (`time`);
		";
		command.ExecuteNonQuery();
	}

	private void InitCommands(SqliteConnection connection)
	{
		_commandGetAllRecordedGroups = connection.CreateCommand();
		_commandGetAllRecordedGroups.CommandText = "SELECT `uin`, `name`, `remark` FROM `group`;";
		_commandGetAllRecordedGroups.Prepare();

		_commandOnGetAllJoinedGroups = connection.CreateCommand();
		_commandOnGetAllJoinedGroups.CommandText = @"
			INSERT OR REPLACE INTO `group` (`uin`, `name`, `remark`)
			VALUES ($uin, $name, $remark);
		";
		_commandOnGetAllJoinedGroups.Parameters.Add(new SqliteParameter("$uin", SqliteType.Integer));
		_commandOnGetAllJoinedGroups.Parameters.Add(new SqliteParameter("$name", SqliteType.Text));
		_commandOnGetAllJoinedGroups.Parameters.Add(new SqliteParameter("$remark", SqliteType.Text));
		_commandOnGetAllJoinedGroups.Prepare();

		_commandGetAllRecordedUsers = connection.CreateCommand();
		_commandGetAllRecordedUsers.CommandText = "SELECT `uin`, `name`, `remark` FROM `user`;";
		_commandGetAllRecordedUsers.Prepare();

		_commandOnGetAllFriends = connection.CreateCommand();
		_commandOnGetAllFriends.CommandText = @"
			INSERT OR REPLACE INTO `user` (`uin`, `name`, `remark`)
			VALUES ($uin, $name, $remark);
		";
		_commandOnGetAllFriends.Parameters.Add(new SqliteParameter("$uin", SqliteType.Integer));
		_commandOnGetAllFriends.Parameters.Add(new SqliteParameter("$name", SqliteType.Text));
		_commandOnGetAllFriends.Parameters.Add(new SqliteParameter("$remark", SqliteType.Text));
		_commandOnGetAllFriends.Prepare();
	}

	private SqliteCommand? _commandGetAllRecordedGroups;

	public override RecordedGroupInfo[] GetAllRecordedGroups()
	{
		var command = _commandGetAllRecordedGroups ?? throw new InvalidOperationException("Command not initialized.");
		using var reader = command.ExecuteReader();
		var groups = new List<RecordedGroupInfo>();
		while (reader.Read())
		{
			groups.Add(new RecordedGroupInfo()
			{
				Uin = reader.GetFieldValue<ulong>(0),
				Name = reader.GetFieldValue<string>(1),
				Remark = reader.IsDBNull(2) ? null : reader.GetFieldValue<string?>(2),
			});
		}
		return [.. groups];
	}

	private SqliteCommand? _commandGetAllRecordedUsers;

	public override RecordedUserInfo[] GetAllRecordedUsers()
	{
		var command = _commandGetAllRecordedUsers ?? throw new InvalidOperationException("Command not initialized.");
		using var reader = command.ExecuteReader();
		var groups = new List<RecordedUserInfo>();
		while (reader.Read())
		{
			groups.Add(new RecordedUserInfo()
			{
				Uin = reader.GetFieldValue<ulong>(0),
				Nickname = reader.GetFieldValue<string>(1),
				Remark = reader.IsDBNull(2) ? null : reader.GetFieldValue<string?>(2),
			});
		}
		return [.. groups];
	}

	#region 事件处理

	private SqliteCommand? _commandOnGetAllFriends;

	private void OnCachedGetAllFriends(object? sender, BusEventArgs<CommonEventId, CachedUserInfo[]> e)
	{
		var command = _commandOnGetAllFriends ?? throw new InvalidOperationException("Command not initialized.");
		foreach (var user in e.Result)
		{
			command.Parameters[0].Value = user.Uin;
			command.Parameters[1].Value = user.Nickname;
			command.Parameters[2].Value = user.Remark != null ? user.Remark : DBNull.Value;
			command.ExecuteNonQuery();
			user.HasLocalData = true;
		}
	}

	private SqliteCommand? _commandOnGetAllJoinedGroups;

	private void OnCachedGetAllJoinedGroups(object? sender, BusEventArgs<CommonEventId, CachedGroupInfo[]> e)
	{
		var command = _commandOnGetAllJoinedGroups ?? throw new InvalidOperationException("Command not initialized.");
		foreach (var group in e.Result)
		{
			command.Parameters[0].Value = group.Uin;
			command.Parameters[1].Value = group.Name;
			command.Parameters[2].Value = group.Remark != null ? group.Remark : DBNull.Value;
			command.ExecuteNonQuery();
			group.HasLocalData = true;
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
				_commandGetAllRecordedGroups?.Dispose();
				_commandOnGetAllJoinedGroups?.Dispose();
				_connection?.Dispose();
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
