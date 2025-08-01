using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaQQ.Events;
using AvaQQ.Exceptions;
using AvaQQ.SDK;
using AvaQQ.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AvaQQ;

internal partial class App : AppBase
{
	private readonly ILogger<App> _logger;

	private readonly IAppLifetime _lifetime;

	private readonly IConnectWindowProvider _connectWindowProvider;

	private readonly IPluginManager _pluginManager;

	private readonly IServiceProvider _serviceProvider;

	private readonly AvaQQEvents _events;

	public override IServiceProvider Services => _serviceProvider;

	public new AppViewModel DataContext
	{
		get => (AppViewModel)(base.DataContext ?? throw new NotInitializedException(nameof(DataContext)));
		set => base.DataContext = value;
	}

	public App(
		AppViewModel viewModel,
		ILogger<App> logger,
		IAppLifetime lifetime,
		IConnectWindowProvider connectWindowProvider,
		IPluginManager pluginManager,
		IServiceProvider serviceProvider,
		AvaQQEvents events
		)
	{
		if (Design.IsDesignMode)
		{
			DesignerHelper.Services = serviceProvider;
		}

		_logger = logger;
		_lifetime = lifetime;
		_connectWindowProvider = connectWindowProvider;
		_pluginManager = pluginManager;
		_serviceProvider = serviceProvider;
		_events = events;

		DataContext = viewModel;
		RegisterUnhandledExceptionHandlers();
	}

	public App() : this(
		DesignerHelper.Services.GetRequiredService<AppViewModel>(),
		DesignerHelper.Services.GetRequiredService<ILogger<App>>(),
		DesignerHelper.Services.GetRequiredService<IAppLifetime>(),
		DesignerHelper.Services.GetRequiredService<IConnectWindowProvider>(),
		DesignerHelper.Services.GetRequiredService<IPluginManager>(),
		DesignerHelper.Services,
		DesignerHelper.Services.GetRequiredService<AvaQQEvents>()
		)
	{
	}

	public override void Initialize()
	{
		_logger.LogInformation("Initializing app...");
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		base.OnFrameworkInitializationCompleted();

		_pluginManager.PreloadPlugins();
		_pluginManager.LoadPlugins();
		_pluginManager.PostLoadPlugins();

		_connectWindowProvider.OpenConnectWindow();
	}

	private void NativeMenuItemExit_Click(object? sender, EventArgs e)
	{
		_logger.LogInformation("App exiting...");

		_lifetime.Shutdown();
	}

	private void TrayIcon_Clicked(object? sender, EventArgs e)
	{
		_events.OnTrayIconClicked.Invoke(EmptyEventResult.Default);
	}

	private void RegisterUnhandledExceptionHandlers()
	{
		AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
		{
			_logger.LogCritical(e.ExceptionObject as Exception, "Unhandled exception in AppDomain!");
			if (e.IsTerminating)
			{
				_lifetime.Shutdown();
			}
		};
		TaskScheduler.UnobservedTaskException += (sender, e) =>
		{
			_logger.LogCritical(e.Exception, "Unobserved task exception!");
			e.SetObserved();
		};
	}
}
