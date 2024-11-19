using System;
using System.IO;
using System.Text.Json;

namespace AvaQQ.SDK;

/// <summary>
/// Constants.
/// </summary>
public static class Constants
{
	/// <summary>
	/// Storage directory.
	/// </summary>
	public static string StorageDirectory { get; } = Directory.Exists("AvaQQ")
			&& File.Exists(".gitignore")
			&& File.Exists("AvaQQ.sln")
			? ".temp"
			: ".";

	/// <summary>
	/// Connection timeout.
	/// </summary>
	public static TimeSpan ConnectionSpan { get; set; } = TimeSpan.FromSeconds(10);

	/// <summary>
	/// Configuration serialization options.
	/// </summary>
	public static JsonSerializerOptions ConfigSerializationOptions { get; } = new()
	{
		WriteIndented = true
	};
}
