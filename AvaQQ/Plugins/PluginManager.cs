using AvaQQ.Resources;
using AvaQQ.SDK;
using AvaQQ.SDK.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text.Json;

namespace AvaQQ.Plugins;

internal class PluginManager : IPluginManager
{
	private static readonly string _rootDirectory = Constants.RootDirectory;

	private static readonly string _pluginsDirectory = Path.Combine(_rootDirectory, "plugins");

	private static readonly string _pluginsInfoFilePath = Path.Combine(_pluginsDirectory, "plugins.json");

	private PluginInfos _pluginInfos = [];

	#region Preload

	public void PreloadPlugins(IHostBuilder hostBuilder)
	{
		try
		{
			FileLoggingExecutor.Information<PluginManager>("Preloading plugins...");

			if (!Directory.Exists(_pluginsDirectory))
			{
				Directory.CreateDirectory(_pluginsDirectory);
			}

			LoadPluginInfos();

			DiscoverPlugins();
			CleanPluginInfos();
			LoadSearchingPaths();
			LoadPluginAssemblies();
			InvokePluginPreLoad(hostBuilder);

			SavePluginInfos();
		}
		catch (Exception e)
		{
			FileLoggingExecutor.Error<PluginManager>(e, "Error while preloading plugins.");
		}
	}

	private void LoadPluginInfos()
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

	private void CleanPluginInfos()
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

	private void SavePluginInfos()
	{
		File.WriteAllText(_pluginsInfoFilePath, JsonSerializer.Serialize(_pluginInfos, Constants.ConfigSerializationOptions));
	}

	private void DiscoverPlugins()
	{
		foreach (var pluginDirectory in Directory.GetDirectories(_pluginsDirectory))
		{
			DiscoverPlugin(pluginDirectory);
		}
	}

	private void DiscoverPlugin(string directory)
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

			FileLoggingExecutor.Information<PluginManager>($"Plugin discovered: [{info.Detail.Id}]{info.Detail.Name}.");
		}
		catch (Exception e)
		{
			FileLoggingExecutor.Error<PluginManager>(e, $"Failed to discover plugin in \"{directory}\".");
		}
	}

	private void LoadPluginAssemblies()
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

	private void LoadPluginAssembly(PluginInfo pluginInfo, string path)
	{
		try
		{
			FileLoggingExecutor.Information<PluginManager>($"Loading assembly: {path}.");

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
			FileLoggingExecutor.Error<PluginManager>(e, $"Failed to load plugin assembly \"{path}\".");
		}
	}

	private void InvokePluginPreLoad(IHostBuilder hostBuilder)
	{
		FileLoggingExecutor.Information<PluginManager>($"Invoking plugin {nameof(Plugin.OnPreLoad)}...");

		foreach (var (_, pluginInfo) in _pluginInfos)
		{
			foreach (var instance in pluginInfo.PluginInstances)
			{
				try
				{
					instance.OnPreLoad(hostBuilder);
				}
				catch (Exception e)
				{
					FileLoggingExecutor.Error<PluginManager>(e, $"Error while invoking plugin method `{nameof(Plugin.OnPreLoad)}` in `{instance.GetType().FullName}`.");
				}
			}
		}
	}

	#endregion

	#region Assembly Resolving

	private readonly AssemblyLoadContext _context = AssemblyLoadContext.Default;

	public PluginManager()
	{
		_context.Resolving += Resolving;
		_context.ResolvingUnmanagedDll += ResolvingUnmanagedDll;
	}

	private readonly List<AssemblyDependencyResolver> _resolvers = [];

	private Assembly? Resolving(AssemblyLoadContext context, AssemblyName assemblyName)
	{
		foreach (var resolver in _resolvers)
		{
			var path = resolver.ResolveAssemblyToPath(assemblyName);
			if (path is not null)
			{
				FileLoggingExecutor.Information<PluginManager>($"Loading dependent assembly: {path}.");

				return _context.LoadFromAssemblyPath(path);
			}
		}

		return null;
	}

	private IntPtr ResolvingUnmanagedDll(Assembly assembly, string unmanagedDllName)
	{
		foreach (var resolver in _resolvers)
		{
			var path = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
			if (path is not null)
			{
				FileLoggingExecutor.Information<PluginManager>($"Loading dependent unmanaged dll: {path}.");

				return NativeLibrary.Load(path);
			}
		}

		return IntPtr.Zero;
	}

	private void LoadSearchingPaths()
	{
		List<string> paths = [typeof(PluginManager).Assembly.Location];
		List<string> cache = []; // 检测循环引用

		LoadSearchingPaths(paths, cache);

		_resolvers.Clear();
		foreach (var path in paths)
		{
			_resolvers.Add(new(path));
		}
	}

	private void LoadSearchingPaths(
		List<string> paths,
		List<string> cache)
	{
		foreach (var (id, pluginInfo) in _pluginInfos)
		{
			try
			{
				LoadSearchingPath(paths, cache, id, pluginInfo);
			}
			catch (Exception e)
			{
				FileLoggingExecutor.Error<PluginManager>(e, $"Failed to load searching directory \"{id}\".");
			}
		}
	}

	private void LoadSearchingPath(
		List<string> paths,
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

			LoadSearchingPath(paths, cache, dependency.Id, dependencyPluginInfo);
		}

		foreach (var assembly in pluginInfo.Detail.Assemblies)
		{
			var path = Path.Combine(pluginInfo.Directory, assembly);
			if (!paths.Contains(path))
			{
				paths.Add(path);
			}
		}

		cache.Remove(id);
	}

	#endregion

	#region Load

	public void LoadPlugins(IServiceProvider serviceProvider)
	{
		var logger = serviceProvider.GetRequiredService<ILogger<PluginManager>>();

		logger.LogInformation($"Invoking plugin {nameof(Plugin.OnLoad)}...");

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

	public void PostLoadPlugins(IServiceProvider serviceProvider)
	{
		var logger = serviceProvider.GetRequiredService<ILogger<PluginManager>>();

		logger.LogInformation($"Invoking plugin {nameof(Plugin.OnPostLoad)}...");

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

	public void UnloadPlugins(IServiceProvider serviceProvider)
	{
		var logger = serviceProvider.GetRequiredService<ILogger<PluginManager>>();

		logger.LogInformation($"Invoking plugin {nameof(Plugin.OnUnload)}...");

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
