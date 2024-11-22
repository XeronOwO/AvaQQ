using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaQQ.Plugins;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.ViewModels;
using AvaQQ.ViewModels.MainPanels;
using AvaQQ.Views.Connecting;
using AvaQQ.Views.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AvaQQ;

public partial class App : AppBase
{
	public App()
	{
		DataContext = new AppViewModel();
	}

	// 受制于 Avalonia Designer 的工作方式，
	// 必须手动设置 ServiceProvider 和 Lifetime，
	// 而且必须要有无参构造函数
	public override IServiceProvider ServiceProvider { get; set; } = null!;

	public override ILifetimeController Lifetime { get; set; } = null!;

	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		base.OnFrameworkInitializationCompleted();

		PluginManager.LoadPlugins(ServiceProvider);

		//if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		//{
		//	desktop.MainWindow = window;
		//}
		//else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
		//{
		//	singleViewPlatform.MainView = window;
		//}

		ConnectWindow = new ConnectWindow()
		{
			DataContext = new ConnectViewModel(),
		};
		ConnectWindow.Show();

		ServiceProvider.GetRequiredService<ILifetimeController>().Stopping += App_Stopping;
	}

	private void App_Stopping(object? sender, EventArgs e)
	{
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

	public override Window? ConnectWindow { get; set; }

	public override Window? MainPanelWindow { get; set; }

	public void NativeMenuItemExit_Click(object? sender, EventArgs e)
	{
		Lifetime.Stop();
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
