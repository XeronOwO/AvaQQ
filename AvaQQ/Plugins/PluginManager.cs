using AvaQQ.SDK;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Loader;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AvaQQ.Plugins;

internal static class PluginManager
{
	private static readonly AssemblyLoadContext _context = AssemblyLoadContext.Default;

	private static readonly string _directory = Path.GetDirectoryName(typeof(PluginManager).Assembly.Location)!;

	private static readonly string _pluginsDirectory = Path.Combine(_directory, "plugins");

	private const string DefaultResolverName = nameof(AvaQQ);

	private static readonly PluginDependencyResolver _defaultResolver = new(DefaultResolverName, _directory);

	private static Dictionary<string, PluginDependencyResolver> _resolvers = [];

	private static readonly string _statusFilePath = Path.Combine(_pluginsDirectory, "plugins.json");

	private static PluginStatusInfos LoadPluginStatuses()
	{
		if (File.Exists(_statusFilePath))
		{
			return JsonSerializer.Deserialize<PluginStatusInfos>(File.ReadAllText(_statusFilePath))
				?? [];
		}
		else
		{
			return [];
		}
	}

	private static void SavePluginStatuses(PluginStatusInfos statuses)
	{
		File.WriteAllText(_statusFilePath, JsonSerializer.Serialize(statuses, Constants.ConfigSerializationOptions));
	}

	public static void LoadPlugins()
	{
		if (!Directory.Exists(_pluginsDirectory))
		{
			Directory.CreateDirectory(_pluginsDirectory);
		}

		_resolvers = new()
		{
			[DefaultResolverName] = _defaultResolver,
		};

		var statuses = LoadPluginStatuses();

		List<string>[] pluginDirectoryLists = [
			[..Directory.GetDirectories(_pluginsDirectory)],
			[],
		];
		var index = 0;
		while (pluginDirectoryLists[index].Count > 0)
		{
			pluginDirectoryLists[1 - index].Clear();
			foreach (var pluginDirectory in pluginDirectoryLists[index])
			{
				if (TryLoadPlugin(statuses, pluginDirectory))
				{

				}
				else
				{
					pluginDirectoryLists[1 - index].Add(pluginDirectory);
				}
			}
		}

		SavePluginStatuses(statuses);
	}

	private static bool TryLoadPlugin(
		PluginStatusInfos statuses,
		string directory)
	{
		var pluginInfoPath = Path.Combine(directory, "plugin.json");
		if (!File.Exists(pluginInfoPath))
		{
			return true; // returns true to skip this plugin
		}

		var pluginInfo = JsonSerializer.Deserialize<PluginInfo>(File.ReadAllText(pluginInfoPath));
		if (pluginInfo is null)
		{
			return true;
		}
		if (string.IsNullOrEmpty(pluginInfo.Name))
		{
			return true;
		}

		return true;
	}
}
