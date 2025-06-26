using Microsoft.EntityFrameworkCore;

namespace AvaQQ.Core.Databases;

internal class DatabaseContext(ulong uin) : DbContext
{
	public string Path { get; } = System.IO.Path.Combine(IDatabase.BaseDirectory, $"user-{uin}.db");

	protected override void OnConfiguring(DbContextOptionsBuilder options)
		=> options.UseSqlite($"Data Source={Path}");

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<RecordedMessage>()
			.HasMany(e => e.TextSegments)
			.WithOne(e => e.Message)
			.HasForeignKey(e => new { e.GroupUin, e.SenderUin, e.Sequence })
			.HasPrincipalKey(e => new { e.GroupUin, e.SenderUin, e.Sequence })
			.OnDelete(DeleteBehavior.Cascade);
	}

	public DbSet<RecordedGroupInfo> Groups { get; set; }

	public DbSet<RecordedUserInfo> Users { get; set; }

	public DbSet<RecordedMessage> Messages { get; set; }

	public DbSet<RecordedTextSegment> TextSegments { get; set; }
}
