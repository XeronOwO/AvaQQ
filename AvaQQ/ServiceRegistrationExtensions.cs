using AvaQQ.Adapters;
using AvaQQ.Caches;
using AvaQQ.Contexts;
using AvaQQ.Databases;
using AvaQQ.Events;
using AvaQQ.Logging;
using AvaQQ.Plugins;
using AvaQQ.SDK;
using AvaQQ.ViewModels;
using AvaQQ.ViewModels.Connect;
using AvaQQ.ViewModels.Main;
using AvaQQ.Views.Connect;
using AvaQQ.Views.Main;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace AvaQQ;

public static class ServiceRegistrationExtensions
{
	public static ILoggingBuilder ConfigureAvaQQLogger(this ILoggingBuilder builder)
	{
		builder.ClearProviders();
		builder.AddConfiguration();
		builder.Services.TryAddEnumerable(
			ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider>()
		);
		LoggerProviderOptions.RegisterProviderOptions<LoggerFilterOptions, LoggerProvider>(builder.Services);
		return builder;
	}

	public static IHostBuilder ConfigureAvaQQ(this IHostBuilder hostBuilder)
		=> hostBuilder.ConfigureServices((context, services) =>
		{
			// Internal services
			services.AddSingleton<AvaQQEvents>();
			services.AddSingleton<AvatarCache>();
			services.AddSingleton<UserCache>();
			services.AddSingleton<LogToFileExecutor>();

			// SDK services
			services.AddSingleton<IAdapterProvider, AdapterProvider>();
			services.AddSingleton<IAdapterViewProvider, AdapterViewProvider>();
			services.AddSingleton<IAppLifetime, AppLifetime>();
			services.AddSingleton<ICacheContext, CacheContext>();
			services.AddSingleton<IConnectWindowProvider, ConnectWindowProvider>();
			services.AddSingleton<IDatabase, SqliteDatabase>();
			services.AddSingleton<IEventBusProvider, EventBusProvider>();
			services.AddSingleton<IMainWindowProvider, MainWindowProvider>();
			services.AddSingleton<IPluginManager, PluginManager>();

			// Avalonia services
			services.AddSingleton<AppBase, App>();
			services.AddTransient<AppViewModel>();

			services.AddScoped<ConnectWindow>();
			services.AddTransient<ConnectView>();
			services.AddTransient<ConnectViewModel>();
			services.AddTransient<ConnectWindowViewModel>();

			services.AddScoped<MainWindow>();
			services.AddTransient<EntryView>();
			services.AddTransient<EntryViewModel>();
			services.AddTransient<EntryListView>();
			services.AddTransient<MainView>();
			services.AddTransient<MainViewModel>();
			services.AddTransient<MainWindowViewModel>();
		});
}
