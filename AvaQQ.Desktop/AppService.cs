using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Svg.Skia;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaQQ.Desktop;

internal class AppService(IServiceProvider serviceProvider, IHost host, ILifetimeController lifetime) : BackgroundService
{
	public AppBuilder BuildAvaloniaApp()
	{
		// https://github.com/wieslawsoltes/Svg.Skia?tab=readme-ov-file#avalonia-previewer
		// To make svg controls work with Avalonia Previewer
		GC.KeepAlive(typeof(SvgImageExtension).Assembly);
		GC.KeepAlive(typeof(Avalonia.Svg.Skia.Svg).Assembly);

		return AppBuilder.Configure(() =>
			{
				var app = (App)serviceProvider.GetRequiredService<AppBase>();
				// Due to the way Avalonia Designer works,
				// we have to set the ServiceProvider and Lifetime manually
				app.ServiceProvider = serviceProvider;
				app.Lifetime = lifetime;
				return app;
			})
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.UseReactiveUI();
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		return Task.Run(() =>
		{
			BuildAvaloniaApp().Start((Application application, string[] args) =>
			{
				if (application is App app)
				{
#pragma warning disable CS0618
					app.Run(lifetime.CancellationTokenSource.Token);
#pragma warning restore CS0618
					_ = host.StopAsync();
				}
			}, Environment.GetCommandLineArgs());
		}, stoppingToken);
	}
}
