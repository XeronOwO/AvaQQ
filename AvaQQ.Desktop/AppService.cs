using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaQQ.Desktop;

internal class AppService(
	IServiceProvider serviceProvider,
	IHost host,
	IAppLifetimeController lifetime
	) : BackgroundService
{
	private AppBuilder BuildAvaloniaApp()
	{
		return AppBuilder.Configure(() => (App)serviceProvider.GetRequiredService<AppBase>())
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.UseReactiveUI();
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		DesignerServiceProviderHelper.Root = serviceProvider;

		return Task.Run(() =>
		{
			BuildAvaloniaApp().Start((Application application, string[] args) =>
			{
				if (application is App app)
				{
					app.Run(lifetime.CancellationTokenSource.Token);
					_ = host.StopAsync();
				}
			}, Environment.GetCommandLineArgs());
		}, stoppingToken);
	}
}
