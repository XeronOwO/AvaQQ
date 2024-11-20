using Avalonia.Controls;
using System;
using System.Text.Json;

namespace AvaQQ.SDK;

/// <summary>
/// 常量
/// </summary>
public static class Constants
{
	/// <summary>
	/// 存储目录
	/// </summary>
	public static string StorageDirectory { get; } = Design.IsDesignMode ? ".design" : ".";

	/// <summary>
	/// 链接超时时间
	/// </summary>
	public static TimeSpan ConnectionSpan { get; set; } = TimeSpan.FromSeconds(10);

	/// <summary>
	/// 配置存储序列化选项
	/// </summary>
	public static JsonSerializerOptions ConfigSerializationOptions { get; } = new()
	{
		WriteIndented = true
	};
}
