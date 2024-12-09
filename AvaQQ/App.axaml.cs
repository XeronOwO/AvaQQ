using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaQQ.Plugins;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.ViewModels;
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

	private readonly IAppLifetimeController _lifetimeController;

	public App(IServiceProvider serviceProvider)
		: base(serviceProvider.GetRequiredService<ILogger<AppBase>>())
	{
		_serviceProvider = serviceProvider;
		_lifetimeController = serviceProvider.GetRequiredService<IAppLifetimeController>();

		DataContext = new AppViewModel();
	}

	public App() : this(DesignerServiceProviderHelper.Root)
	{
	}

	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	private IServiceScope? _connectServiceScope;

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

		_connectServiceScope = _serviceProvider.CreateScope();
		ConnectWindow = _connectServiceScope.ServiceProvider.GetRequiredService<ConnectWindowBase>();
		ConnectWindow.Show();

		_lifetimeController.Stopping += App_Stopping;
	}

	private void App_Stopping(object? sender, EventArgs e)
	{
		PluginManager.UnloadPlugins(_serviceProvider);

		if (Adapter is not null)
		{
			Adapter.Dispose();
			Adapter = null;
		}

		if (ConnectWindow is not null)
		{
			ConnectWindow.Close();
			ConnectWindow = null;
		}

		if (MainPanelWindow is not null)
		{
			MainPanelWindow.Close();
			MainPanelWindow = null;
		}
	}

	public override IAdapter? Adapter { get; set; }

	public override ConnectWindowBase? ConnectWindow { get; set; }

	public override Window? MainPanelWindow { get; set; }

	public void NativeMenuItemExit_Click(object? sender, EventArgs e)
	{
		_lifetimeController.Stop();
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
		if (ConnectWindow is not { } connect)
		{
			return;
		}

		connect.Activate();
	}

	private void TryOpenOrFocusMainPanelWindow()
	{
		if (MainPanelWindow is null)
		{
			MainPanelWindow = new MainPanelWindow()
			{
				DataContext = new MainPanelViewModel(),
			};
			MainPanelWindow.Show();
		}
		else
		{
			MainPanelWindow.Activate();
		}
	}
}
