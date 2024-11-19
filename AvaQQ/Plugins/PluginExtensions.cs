using Microsoft.Extensions.Hosting;

namespace AvaQQ.Plugins;

public static class PluginExtensions
{
	public static IHostBuilder ConfigurePlugins(this IHostBuilder host)
	{
		PluginManager.PreloadPlugins(host);
		return host;
	}
}
