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
				// 受制于 Avalonia Designer 的工作方式，
				// 必须手动设置 ServiceProvider 和 Lifetime，
				// 而且必须要有无参构造函数
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
					app.Run(lifetime.CancellationTokenSource.Token);
					_ = host.StopAsync();
				}
			}, Environment.GetCommandLineArgs());
		}, stoppingToken);
	}
}
