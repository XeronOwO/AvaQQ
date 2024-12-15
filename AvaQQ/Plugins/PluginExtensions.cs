using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AvaQQ.Plugins;

public static class PluginExtensions
{
	public static IHostBuilder ConfigurePlugins(this IHostBuilder host)
	{
		var pluginManager = new PluginManager();
		host.ConfigureServices(services =>
		{
			services.AddSingleton<IPluginManager>(pluginManager);
		});
		pluginManager.PreloadPlugins(host);
		return host;
	}
}
