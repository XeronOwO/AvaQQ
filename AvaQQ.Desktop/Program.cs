using Avalonia;
using Avalonia.ReactiveUI;
using Avalonia.Svg.Skia;
using AvaQQ.Logging;
using AvaQQ.Plugins;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace AvaQQ.Desktop;

internal class Program
{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args)
		=> Host.CreateDefaultBuilder(args)
			.ConfigureLogging(logging =>
				logging.ClearProviders()
					.AddFileLogger()
			)
			.ConfigureServices(services =>
				services.AddAvaQQ()
					.AddHostedService<AppService>()
			)
			.ConfigurePlugins()
			.Build()
			.Run();

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
	{
		// https://github.com/wieslawsoltes/Svg.Skia?tab=readme-ov-file#avalonia-previewer
		// To make svg controls work with Avalonia Previewer
		GC.KeepAlive(typeof(SvgImageExtension).Assembly);
		GC.KeepAlive(typeof(Avalonia.Svg.Skia.Svg).Assembly);

		return AppBuilder.Configure(() =>
			{
				var builder = Host.CreateDefaultBuilder()
					.ConfigureLogging(logging =>
						logging.ClearProviders()
							.AddFileLogger()
					)
					.ConfigureServices(services =>
						services.AddAvaQQ()
							.AddHostedService<AppService>()
					)
					.ConfigurePlugins()
					.Build();

				var app = (App)builder.Services.GetRequiredService<AppBase>();
				// 受制于 Avalonia Designer 的工作方式，
				// 必须手动设置 ServiceProvider 和 Lifetime，
				// 而且必须要有无参构造函数
				app.ServiceProvider = builder.Services;
				app.Lifetime = builder.Services.GetRequiredService<ILifetimeController>();
				return app;
			})
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.UseReactiveUI();
	}
}
