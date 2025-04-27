using Microsoft.EntityFrameworkCore;

namespace AvaQQ.Core.Databases;

internal class UserSqliteContext(ulong uin) : DbContext
{
	public string Path { get; } = System.IO.Path.Combine(Databases.Database.BaseDirectory, $"user-{uin}.db");

	protected override void OnConfiguring(DbContextOptionsBuilder options)
		=> options.UseSqlite($"Data Source={Path}");

	public DbSet<RecordedGroupInfo> Groups { get; set; }

	public DbSet<RecordedUserInfo> Users { get; set; }

	public DbSet<RecordedMessage> Messages { get; set; }

	public DbSet<RecordedTextSegment> TextSegments { get; set; }
}
