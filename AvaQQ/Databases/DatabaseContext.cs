using AvaQQ.Entities;
using AvaQQ.SDK;
using Microsoft.EntityFrameworkCore;

namespace AvaQQ.Databases;

public class DatabaseContext(ulong uin) : DbContext
{
	public string Path { get; } = System.IO.Path.Combine(IDatabase.BaseDirectory, $"user-{uin}.db");

	protected override void OnConfiguring(DbContextOptionsBuilder options)
		=> options.UseSnakeCaseNamingConvention()
			.UseLazyLoadingProxies()
			.UseSqlite($"Data Source={Path}");

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Message>()
			.HasMany(e => e.TextSegments)
			.WithOne(e => e.Message)
			.HasPrincipalKey(e => new { e.GroupUin, e.SenderUin, e.Sequence })
			.HasForeignKey(e => new { e.GroupUin, e.SenderUin, e.Sequence })
			.OnDelete(DeleteBehavior.Cascade);
	}

	public DbSet<GroupInfo> Groups { get; set; }

	public DbSet<UserInfo> Users { get; set; }

	public DbSet<Message> Messages { get; set; }

	public DbSet<TextSegment> TextSegments { get; set; }
}
