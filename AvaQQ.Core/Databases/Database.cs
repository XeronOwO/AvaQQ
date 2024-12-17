using AvaQQ.SDK;

namespace AvaQQ.Core.Databases;

/// <summary>
/// 数据库
/// </summary>
public abstract class Database : IDisposable
{
	/// <summary>
	/// 数据库目录
	/// </summary>
	public static string BaseDirectory { get; } = Path.Combine(Constants.RootDirectory, "db");

	static Database()
	{
		if (!Directory.Exists(BaseDirectory))
		{
			Directory.CreateDirectory(BaseDirectory);
		}
	}

	/// <inheritdoc/>
	public abstract void Dispose();
}
