using Avalonia;
using Avalonia.Controls;
using System.Text.Json;

namespace AvaQQ.SDK;

public static class Constants
{
	public static string RootDirectory { get; } =
		Design.IsDesignMode
		// Avalonia Designer 将输出目录作为根目录
		? Path.GetDirectoryName(typeof(Application).Assembly.Location)!
		// 将工作目录作为根目录，方便定制化部署
		: Environment.CurrentDirectory;

	public static JsonSerializerOptions ConfigSerializationOptions { get; } = new()
	{
		WriteIndented = true
	};
}
