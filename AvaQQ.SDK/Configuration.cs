using AvaQQ.SDK.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;

namespace AvaQQ.SDK;

/// <summary>
/// 配置管理器
/// </summary>
public abstract class Configuration
{
	private static readonly string _baseDirectory;

	/// <summary>
	/// 配置文件存储目录
	/// </summary>
	public static string BaseDirectory => _baseDirectory;

	static Configuration()
	{
		_baseDirectory = Path.Combine(Constants.RootDirectory, "config");
		Directory.CreateDirectory(_baseDirectory);
	}
}

/// <summary>
/// 配置管理器
/// </summary>
/// <typeparam name="T">配置数据结构</typeparam>
public class Configuration<T> : Configuration
	where T : class, new()
{
	private const string IgnoredNameSuffix = "Configuration";

	private static string? _name;

	/// <summary>
	/// 获取或设置配置文件的名称。<br/>
	/// 默认值为类型的名称，去除后缀 "Configuration" 并转换为蛇形命名法。
	/// </summary>
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

	/// <summary>
	/// 获取配置文件的路径。
	/// </summary>
	public static string ConfigPath { get; } = Path.Combine(BaseDirectory, Name);

	private static Lazy<T> _lazyInstance;

	/// <summary>
	/// 获取配置的实例。
	/// </summary>
	public static T Instance => _lazyInstance.Value;

	static Configuration()
	{
		Reload();
	}

	/// <summary>
	/// 重新加载配置。
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
	/// 保存配置。
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
