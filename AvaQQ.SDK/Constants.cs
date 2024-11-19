using Avalonia.Controls;
using System;
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
	public static string StorageDirectory { get; } = Design.IsDesignMode ? ".design" : ".";

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
