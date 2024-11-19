using AvaQQ.Logging;
using AvaQQ.Plugins;
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
}
