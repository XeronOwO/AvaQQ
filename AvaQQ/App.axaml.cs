using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaQQ.Plugins;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.Views;
using AvaQQ.ViewModels;
using AvaQQ.ViewModels.MainPanels;
using AvaQQ.Views.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace AvaQQ;

public partial class App : AppBase
{
	private readonly IServiceProvider _serviceProvider;

	public App(IServiceProvider serviceProvider)
		: base(
			serviceProvider.GetRequiredService<ILogger<AppBase>>(),
			serviceProvider.GetRequiredService<IAppLifetimeController>()
			)
	{
		_serviceProvider = serviceProvider;

		DataContext = new AppViewModel();
	}

	public App() : this(DesignerServiceProviderHelper.Root)
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

		if (MainPanelWindow is not null)
		{
			MainPanelWindow.Close();
			MainPanelWindow = null;
		}
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

		throw new NotImplementedException();
	}

	public override Window? MainPanelWindow { get; set; }

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

		MainWindow = new MainPanelWindow()
		{
			DataContext = new MainPanelViewModel(),
		};
		MainWindow.Show();
	}
}
