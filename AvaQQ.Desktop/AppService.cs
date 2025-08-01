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
	IAppLifetime lifetime,
	IPluginManager pluginManager
	) : BackgroundService
{
	private AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure(serviceProvider.GetRequiredService<AppBase>)
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.UseReactiveUI();

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
		=> Task.Run(() =>
		{
			BuildAvaloniaApp().Start((application, args) =>
			{
				if (application is AppBase app)
				{
					app.Run(lifetime.Token);
					pluginManager.UnloadPlugins();
					// Clean tray icon
					TrayIcons? trays = TrayIcon.GetIcons(application);
					if (trays != null)
					{
						foreach (TrayIcon tray in trays)
						{
							tray.Dispose();
						}
					}
					_ = host.StopAsync();
				}
			}, Environment.GetCommandLineArgs());
		}, stoppingToken);
}
