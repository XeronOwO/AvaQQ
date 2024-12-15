using Avalonia;
using Avalonia.Controls;
using System.Text.Json;

namespace AvaQQ.SDK;

/// <summary>
/// 常量
/// </summary>
public static class Constants
{
	/// <summary>
	/// 根目录
	/// </summary>
	public static string RootDirectory { get; } =
		Design.IsDesignMode
		// Avalonia Designer 使用输出目录作为根目录
		? Path.GetDirectoryName(typeof(Application).Assembly.Location)!
		// 使用工作目录作为根目录，方便定制化部署
		: Environment.CurrentDirectory;

	/// <summary>
	/// 配置存储序列化选项
	/// </summary>
	public static JsonSerializerOptions ConfigSerializationOptions { get; } = new()
	{
		WriteIndented = true
	};
}
