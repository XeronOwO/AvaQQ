using AvaQQ.SDK.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;

namespace AvaQQ.SDK;

public abstract class Configuration
{
	private static readonly string _baseDirectory;

	public static string BaseDirectory => _baseDirectory;

	static Configuration()
	{
		_baseDirectory = Path.Combine(Constants.RootDirectory, "config");
		Directory.CreateDirectory(_baseDirectory);
	}
}

public class Configuration<T> : Configuration
	where T : class, new()
{
	private const string IgnoredNameSuffix = "Configuration";

	private static string? _name;

	[AllowNull]
	public static string Name
	{
		get
		{
			if (_name is null)
			{
				var type = typeof(T);
				if (type.GetCustomAttribute<ConfigurationNameAttribute>() is { } attribute)
				{
					_name = attribute.Name;
					return _name;
				}

				var name = type.Name;
				if (name.EndsWith(IgnoredNameSuffix, StringComparison.OrdinalIgnoreCase))
				{
					name = name[..^IgnoredNameSuffix.Length];
				}
				_name = JsonNamingPolicy.SnakeCaseLower.ConvertName(name) + ".json";
				return _name;
			}

			return _name;
		}
		set => _name = value;
	}

	public static string ConfigPath { get; } = Path.Combine(BaseDirectory, Name);

	private static Lazy<T> _lazyInstance;

	public static T Instance => _lazyInstance.Value;

	static Configuration()
	{
		Reload();
	}

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

	public static void Save()
	{
		SaveInternal(Instance);
	}

	private static void SaveInternal(T instance)
	{
		File.WriteAllText(ConfigPath, JsonSerializer.Serialize(instance, Constants.ConfigSerializationOptions));
	}
}
