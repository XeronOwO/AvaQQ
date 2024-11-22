using Avalonia;
using AvaQQ.Resources;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace AvaQQ.Plugins;

internal class PluginManager
{
	private static readonly string _rootDirectory = Constants.RootDirectory;

	private static readonly string _pluginsDirectory = Path.Combine(_rootDirectory, "plugins");

	private static readonly string _pluginsInfoFilePath = Path.Combine(_pluginsDirectory, "plugins.json");

	private static PluginInfos _pluginInfos = [];

	#region Preload

	public static void PreloadPlugins(IHostBuilder hostBuilder)
	{
		try
		{
			if (!Directory.Exists(_pluginsDirectory))
			{
				Directory.CreateDirectory(_pluginsDirectory);
			}

			LoadPluginInfos();

			DiscoverPlugins();
			CleanPluginInfos();
			LoadSearchingDirectories();
			LoadPluginAssemblies();
			InvokePluginPreLoad(hostBuilder);

			SavePluginInfos();
		}
		catch (Exception e)
		{
			Debug.WriteLine(SR.ErrorErrorWhilePreloadingPlugins);
			Debug.WriteLine(e);
		}
	}

	private static void LoadPluginInfos()
	{
		if (File.Exists(_pluginsInfoFilePath))
		{
			var pluginInfos = JsonSerializer.Deserialize<PluginInfos>(File.ReadAllText(_pluginsInfoFilePath));
			if (pluginInfos is not null)
			{
				_pluginInfos = pluginInfos;
			}
		}
	}

	private static void CleanPluginInfos()
	{
		var pluginInfos = new PluginInfos();
		foreach (var (id, pluginInfo) in _pluginInfos)
		{
			if (pluginInfo.Visited)
			{
				pluginInfos.Add(id, pluginInfo);
			}
		}
		_pluginInfos = pluginInfos;
	}

	private static void SavePluginInfos()
	{
		File.WriteAllText(_pluginsInfoFilePath, JsonSerializer.Serialize(_pluginInfos, Constants.ConfigSerializationOptions));
	}

	private static void DiscoverPlugins()
	{
		foreach (var pluginDirectory in Directory.GetDirectories(_pluginsDirectory))
		{
			DiscoverPlugin(pluginDirectory);
		}
	}

	private static void DiscoverPlugin(string directory)
	{
		try
		{
			var detailInfoFilePath = Path.Combine(directory, "plugin.json");
			if (!File.Exists(detailInfoFilePath))
			{
				return;
			}

			var detail = JsonSerializer.Deserialize<PluginDetailInfo>(File.ReadAllText(detailInfoFilePath));
			if (detail is null)
			{
				return;
			}

			if (!_pluginInfos.TryGetValue(detail.Id, out var info))
			{
				_pluginInfos.Add(detail.Id, info = new());
			}

			info.Visited = true;
			info.Directory = directory;
			info.Detail = detail;
		}
		catch (Exception e)
		{
			Debug.WriteLine(string.Format(SR.ErrorFailedToDiscoverPlugin, directory));
			Debug.WriteLine(e);
		}
	}

	private static void LoadPluginAssemblies()
	{
		foreach (var (id, pluginInfo) in _pluginInfos)
		{
			foreach (var assemblyFilename in pluginInfo.Detail.Assemblies)
			{
				var path = Path.Combine(pluginInfo.Directory, assemblyFilename);
				LoadPluginAssembly(pluginInfo, path);
			}
		}
	}

	private static void LoadPluginAssembly(PluginInfo pluginInfo, string path)
	{
		try
		{
			var assembly = _context.LoadFromAssemblyPath(path);

			var pluginTypes = assembly.GetTypes().Where(t => typeof(Plugin).IsAssignableFrom(t));
			var pluginInstances = new List<Plugin>();
			foreach (var pluginType in pluginTypes)
			{
				var instance = Activator.CreateInstance(pluginType);
				if (instance is null
					|| instance is not Plugin pluginInstance)
				{
					continue;
				}

				pluginInstances.Add(pluginInstance);
			}

			pluginInfo.PluginInstances = [.. pluginInstances];
		}
		catch (Exception e)
		{
			Debug.WriteLine(string.Format(SR.ErrorFailedToLoadPluginAssembly, path));
			Debug.WriteLine(e);
		}
	}

	private static void InvokePluginPreLoad(IHostBuilder hostBuilder)
	{
		foreach (var (_, pluginInfo) in _pluginInfos)
		{
			foreach (var instance in pluginInfo.PluginInstances)
			{
				try
				{
					instance.OnPreload(hostBuilder);
				}
				catch (Exception e)
				{
					Debug.WriteLine(string.Format(
						"Error while invoking plugin method `{0}` in `{1}`.",
						nameof(Plugin.OnPreload),
						instance.GetType().FullName
						));
					Debug.WriteLine(e);
				}
			}
		}
	}

	#endregion

	#region Assembly Resolving

	private static readonly AssemblyLoadContext _context = AssemblyLoadContext.Default;

	static PluginManager()
	{
		_context.Resolving += Resolving;
	}

	private static string[] _searchingDirectories = [];

	private static Assembly? Resolving(AssemblyLoadContext context, AssemblyName assemblyName)
	{
		foreach (var directory in _searchingDirectories)
		{
			var path = Path.Combine(directory, $"{assemblyName.Name}.dll");
			if (File.Exists(path))
			{
				return context.LoadFromAssemblyPath(path);
			}
		}

		return null;
	}

	private static void LoadSearchingDirectories()
	{
		List<string> searchDirectories = [Path.GetDirectoryName(typeof(PluginManager).Assembly.Location)];
		List<string> cache = []; // 检测循环引用

		LoadSearchingDirectories(searchDirectories, cache);

		_searchingDirectories = [.. searchDirectories];
	}

	private static void LoadSearchingDirectories(
		List<string> directories,
		List<string> cache)
	{
		foreach (var (id, pluginInfo) in _pluginInfos)
		{
			try
			{
				LoadSearchingDirectory(directories, cache, id, pluginInfo);
			}
			catch (Exception e)
			{
				Debug.WriteLine(string.Format(SR.ErrorFailedToLoadSearchingDirectory, id));
				Debug.WriteLine(e);
			}
		}
	}

	private static void LoadSearchingDirectory(
		List<string> directories,
		List<string> cache,
		string id,
		PluginInfo pluginInfo)
	{
		if (cache.Contains(id))
		{
			throw new InvalidOperationException(SR.ExceptionCircularReferenceDetected);
		}
		cache.Add(id);

		foreach (var dependency in pluginInfo.Detail.Dependencies)
		{
			if (!_pluginInfos.TryGetValue(dependency.Id, out var dependencyPluginInfo))
			{
				throw new InvalidOperationException(string.Format(SR.ExceptionPluginDependencyNotFound, dependency.Id));
			}

			LoadSearchingDirectory(directories, cache, dependency.Id, dependencyPluginInfo);
		}

		if (!directories.Contains(pluginInfo.Directory))
		{
			directories.Add(pluginInfo.Directory);
		}

		cache.Remove(id);
	}

	#endregion

	#region Load

	public static void LoadPlugins(IServiceProvider serviceProvider)
	{
		var logger = serviceProvider.GetRequiredService<ILogger<PluginManager>>();

		foreach (var (_, pluginInfo) in _pluginInfos)
		{
			foreach (var instance in pluginInfo.PluginInstances)
			{
				try
				{
					instance.OnLoad(serviceProvider);
				}
				catch (Exception e)
				{
					logger.LogError(e,
						"Error while invoking plugin method `{Method}` in `{Plugin}`.",
						nameof(Plugin.OnLoad),
						instance.GetType().FullName
					);
				}
			}
		}
	}

	#endregion

	#region Post Load

	public static void PostLoadPlugins(IServiceProvider serviceProvider)
	{
		var logger = serviceProvider.GetRequiredService<ILogger<PluginManager>>();

		foreach (var (_, pluginInfo) in _pluginInfos)
		{
			foreach (var instance in pluginInfo.PluginInstances)
			{
				try
				{
					instance.OnPostLoad(serviceProvider);
				}
				catch (Exception e)
				{
					logger.LogError(e,
						"Error while invoking plugin method `{Method}` in `{Plugin}`.",
						nameof(Plugin.OnPostLoad),
						instance.GetType().FullName
					);
				}
			}
		}
	}

	#endregion

	#region Unload

	public static void UnloadPlugins(IServiceProvider serviceProvider)
	{
		var logger = serviceProvider.GetRequiredService<ILogger<PluginManager>>();

		foreach (var (_, pluginInfo) in _pluginInfos)
		{
			foreach (var instance in pluginInfo.PluginInstances)
			{
				try
				{
					instance.OnUnload();
				}
				catch (Exception e)
				{
					logger.LogError(e,
						"Error while invoking plugin method `{Method}` in `{Plugin}`.",
						nameof(Plugin.OnUnload),
						instance.GetType().FullName
					);
				}
			}
		}
	}

	#endregion
}
