using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AvaQQ.Desktop;

internal class AppService(
	IServiceProvider serviceProvider,
	IHost host,
	IAppLifetimeController lifetime,
	IPluginManager pluginManager
	) : BackgroundService
{
	private AppBuilder BuildAvaloniaApp()
	{
		return AppBuilder.Configure(serviceProvider.GetRequiredService<AppBase>)
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.UseReactiveUI();
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		//DesignerServiceProviderHelper.Root = serviceProvider;

		return Task.Run(() =>
		{
			pluginManager.LoadPlugins(serviceProvider);
			pluginManager.PostLoadPlugins(serviceProvider);

			BuildAvaloniaApp().Start((Application application, string[] args) =>
			{
				if (application is AppBase app)
				{
					app.Run(lifetime.CancellationToken);
					pluginManager.UnloadPlugins(serviceProvider);
					_ = host.StopAsync();
				}
			}, Environment.GetCommandLineArgs());
		}, stoppingToken);
	}
}
