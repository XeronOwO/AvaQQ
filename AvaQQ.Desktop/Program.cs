using Avalonia.Svg.Skia;
using Avalonia;
using AvaQQ.Logging;
using AvaQQ.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Avalonia.ReactiveUI;
using AvaQQ.SDK;

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
				// Due to the way Avalonia Designer works,
				// we have to set the ServiceProvider and Lifetime manually
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
