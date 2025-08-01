using AvaQQ.SDK.Entities;

namespace AvaQQ.SDK;

public interface IDatabase : IDisposable
{
	public static string BaseDirectory { get; } = Path.Combine(Constants.RootDirectory, "db");

	static IDatabase()
	{
		Directory.CreateDirectory(BaseDirectory);
	}

	public void Initialize(ulong uin);

	public Task<IUserInfo?> GetUserAsync(ulong uin, CancellationToken token);
}
