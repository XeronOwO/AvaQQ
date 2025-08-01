using Avalonia;
using Avalonia.ReactiveUI;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
				logging.ConfigureAvaQQLogger()
			)
			.ConfigureServices(services =>
				services.AddHostedService<AppService>()
			)
			.ConfigureAvaQQ()
			.Build()
			.Run();

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure(() =>
		{
			var host = Host.CreateDefaultBuilder()
				.ConfigureLogging(logging =>
					logging.ConfigureAvaQQLogger()
				)
				.ConfigureServices(services =>
					services.AddHostedService<AppService>()
				)
				.ConfigureAvaQQ()
				.Build();

			return host.Services.GetRequiredService<AppBase>();
		})
		.UsePlatformDetect()
		.WithInterFont()
		.LogToTrace()
		.UseReactiveUI();
}
