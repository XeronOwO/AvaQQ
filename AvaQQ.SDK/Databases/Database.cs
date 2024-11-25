using System;
using System.IO;

namespace AvaQQ.SDK.Databases;

/// <summary>
/// 数据库
/// </summary>
public abstract class Database : IDisposable
{
	/// <summary>
	/// 数据库目录
	/// </summary>
	public static string BaseDirectory { get; } = Path.Combine(Constants.RootDirectory, "db");

	/// <inheritdoc/>
	public abstract void Dispose();
}
