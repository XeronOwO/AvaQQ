﻿using Avalonia.Markup.Xaml;
using AvaQQ.Core.Adapters;
using AvaQQ.Core.Databases;
using AvaQQ.Core.ViewModels;
using AvaQQ.Core.Views.Connecting;
using AvaQQ.Core.Views.MainPanels;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AvaQQ.Core;

internal partial class App : AppBase, IDisposable
{
	private readonly IServiceProvider _serviceProvider;

	private readonly IAdapterProvider _adapterProvider;

	private readonly GroupMessageDatabase _groupMessageDatabase;

	public App(
		IServiceProvider serviceProvider,
		IAdapterProvider adapterProvider,
		GroupMessageDatabase groupMessageDatabase
		) : base(
			serviceProvider.GetRequiredService<ILogger<AppBase>>(),
			serviceProvider.GetRequiredService<IAppLifetimeController>()
			)
	{
		_serviceProvider = serviceProvider;
		_adapterProvider = adapterProvider;
		_groupMessageDatabase = groupMessageDatabase;

		DataContext = new AppViewModel();
	}

	public App() : this(
		DesignerServiceProviderHelper.Root,
		DesignerServiceProviderHelper.Root.GetRequiredService<IAdapterProvider>(),
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

		//if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		//{
		//	desktop.MainWindow = window;
		//}
		//else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
		//{
		//	singleViewPlatform.MainView = window;
		//}

		OpenConnectWindow();
	}

	private void NativeMenuItemExit_Click(object? sender, EventArgs e)
	{
		_lifetime.Stop();
	}

	private void TrayIcon_Clicked(object? sender, EventArgs e)
	{
		// 连接窗口
		if (_connectScope is not null && MainWindow is not null)
		{
			MainWindow.Activate();
			return;
		}

		// 主面板窗口
		if (_mainPanelScope is not null && MainWindow is not null)
		{
			MainWindow.Activate();
		}
		else
		{
			OpenMainPanelWindow();
		}
	}

	private IServiceScope? _connectScope;

	private void OpenConnectWindow()
	{
		_connectScope = _serviceProvider.CreateScope();
		MainWindow = _connectScope.ServiceProvider.GetRequiredService<ConnectWindow>();
		MainWindow.Show();
		MainWindow.Closed += ConnectWindow_Closed;
	}

	private void ConnectWindow_Closed(object? sender, EventArgs e)
	{
		MainWindow = null;
		_connectScope?.Dispose();
		_connectScope = null;

		if (_adapterProvider.Adapter is not { } adapter)
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
		MainWindow.Show();
		MainWindow.Closed += MainPanelWindow_Closed;
	}

	private void MainPanelWindow_Closed(object? sender, EventArgs e)
	{
		MainWindow = null;
		_mainPanelScope?.Dispose();
		_mainPanelScope = null;
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_connectScope?.Dispose();
				_mainPanelScope?.Dispose();
			}

			disposedValue = true;
		}
	}

	~App()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion
}