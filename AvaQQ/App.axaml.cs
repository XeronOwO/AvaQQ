using Avalonia.Markup.Xaml;
using AvaQQ.Plugins;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.Databases;
using AvaQQ.SDK.Views;
using AvaQQ.ViewModels;
using AvaQQ.Views.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace AvaQQ;

public partial class App : AppBase
{
	private readonly IServiceProvider _serviceProvider;

	private readonly GroupMessageDatabase _groupMessageDatabase;

	public App(
		IServiceProvider serviceProvider,
		GroupMessageDatabase groupMessageDatabase
		) : base(
			serviceProvider.GetRequiredService<ILogger<AppBase>>(),
			serviceProvider.GetRequiredService<IAppLifetimeController>()
			)
	{
		_serviceProvider = serviceProvider;
		_groupMessageDatabase = groupMessageDatabase;

		DataContext = new AppViewModel();
	}

	public App() : this(
		DesignerServiceProviderHelper.Root,
		DesignerServiceProviderHelper.Root.GetRequiredService<GroupMessageDatabase>()
		)
	{
	}

	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		base.OnFrameworkInitializationCompleted();

		PluginManager.LoadPlugins(_serviceProvider);
		PluginManager.PostLoadPlugins(_serviceProvider);

		//if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		//{
		//	desktop.MainWindow = window;
		//}
		//else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
		//{
		//	singleViewPlatform.MainView = window;
		//}

		_lifetime.Stopping += App_Stopping;

		OpenConnectWindow();
	}

	private void App_Stopping(object? sender, EventArgs e)
	{
		PluginManager.UnloadPlugins(_serviceProvider);

		if (Adapter is not null)
		{
			Adapter.Dispose();
			Adapter = null;
		}

		_connectScope?.Dispose();

		_mainPanelScope?.Dispose();
	}

	private IServiceScope? _connectScope;

	private void OpenConnectWindow()
	{
		_connectScope = _serviceProvider.CreateScope();
		MainWindow = _connectScope.ServiceProvider.GetRequiredService<ConnectWindowBase>();
		MainWindow.Show();
	}

	public override void OnConnected(IAdapter? adapter)
	{
		Adapter = adapter;

		_connectScope?.Dispose();
		_connectScope = null;
		MainWindow = null;

		if (adapter is null)
		{
			_lifetime.Stop();
			return;
		}

		_groupMessageDatabase.Initialize(adapter);

		OpenMainPanelWindow();
	}

	private IServiceScope? _mainPanelScope;

	private void OpenMainPanelWindow()
	{
		_mainPanelScope = _serviceProvider.CreateScope();
		MainWindow = _mainPanelScope.ServiceProvider.GetRequiredService<MainPanelWindow>();
		MainWindow.Closed += (_, _) =>
		{
			MainWindow = null;
			_mainPanelScope?.Dispose();
			_mainPanelScope = null;
		};
		MainWindow.Show();
	}

	public void NativeMenuItemExit_Click(object? sender, EventArgs e)
	{
		_lifetime.Stop();
	}

	public void TrayIcon_Clicked(object? sender, EventArgs e)
	{
		if (Adapter is null)
		{
			TryFocusConnectWindow();
		}
		else
		{
			TryOpenOrFocusMainPanelWindow();
		}
	}

	private void TryFocusConnectWindow()
	{
		MainWindow?.Activate();
	}

	private void TryOpenOrFocusMainPanelWindow()
	{
		if (MainWindow is not null)
		{
			MainWindow.Activate();
			return;
		}

		OpenMainPanelWindow();
	}
}
