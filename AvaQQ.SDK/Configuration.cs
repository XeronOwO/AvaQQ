using AvaQQ.SDK.Resources;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace AvaQQ.SDK;

/// <summary>
/// Represents a configuration manager.
/// </summary>
/// <typeparam name="T">Configuration data</typeparam>
public class Configuration<T>
	where T : class, new()
{
	private static readonly string _baseDirectory;

	private const string IgnoredNameSuffix = "Configuration";

	private static string? _name;

	/// <summary>
	/// Gets or sets the name of the configuration file.<br/>
	/// Default value is the name of the type with the suffix "Configuration" removed and converted to snake case.
	/// </summary>
	[AllowNull]
	public static string Name
	{
		get
		{
			if (_name is null)
			{
				var name = typeof(T).Name;
				if (name.EndsWith(IgnoredNameSuffix, StringComparison.OrdinalIgnoreCase))
				{
					name = name[..^IgnoredNameSuffix.Length];
				}
				_name = JsonNamingPolicy.SnakeCaseLower.ConvertName(name) + ".json";
			}

			return _name;
		}
		set => _name = value;
	}

	/// <summary>
	/// Gets the path of the configuration file.
	/// </summary>
	public static string ConfigPath => Path.Combine(_baseDirectory, Name);

	private static Lazy<T> _lazyInstance;

	/// <summary>
	/// Gets the instance of the configuration.
	/// </summary>
	public static T Instance => _lazyInstance.Value;

	static Configuration()
	{
		_baseDirectory = Path.Combine(Constants.StorageDirectory, "config");

		if (!Directory.Exists(_baseDirectory))
		{
			Directory.CreateDirectory(_baseDirectory);
		}

		Reload();
	}

	/// <summary>
	/// Reloads the configuration.
	/// </summary>
	[MemberNotNull(nameof(_lazyInstance))]
	public static void Reload()
	{
		_lazyInstance = new(LoadOrCreateInternal);
	}

	private static T LoadOrCreateInternal()
	{
		var path = ConfigPath;

		T result;
		if (!File.Exists(path))
		{
			result = new T();
		}
		else
		{
			result = JsonSerializer.Deserialize<T>(File.ReadAllText(path))
				?? throw new InvalidOperationException(string.Format(SR.ExceptionFailedToLoadConfiguration, path));
		}

		SaveInternal(result);
		return result;
	}

	/// <summary>
	/// Saves the configuration.
	/// </summary>
	public static void Save()
	{
		SaveInternal(Instance);
	}

	private static void SaveInternal(T instance)
	{
		File.WriteAllText(ConfigPath, JsonSerializer.Serialize(instance, Constants.ConfigSerializationOptions));
	}
}
