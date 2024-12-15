using AvaQQ.Core.Adapters;
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

	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="adapter">适配器</param>
	public abstract void Initialize(IAdapter adapter);

	/// <inheritdoc/>
	public abstract void Dispose();
}
