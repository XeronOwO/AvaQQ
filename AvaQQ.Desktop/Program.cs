using Avalonia;
using Avalonia.ReactiveUI;
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
				DesignerServiceProviderHelper.Root = builder.Services.GetRequiredService<IServiceProvider>();

				return (App)builder.Services.GetRequiredService<AppBase>();
			})
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.UseReactiveUI();
	}
}
