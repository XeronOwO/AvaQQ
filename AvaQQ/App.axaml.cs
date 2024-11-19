using Avalonia.Markup.Xaml;
using AvaQQ.Plugins;
using AvaQQ.Resources;
using AvaQQ.SDK;
using AvaQQ.ViewModels;
using AvaQQ.Views.Connecting;
using AvaQQ.Views.MainPanels;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AvaQQ;

public partial class App(
	IServiceProvider serviceProvider,
	ILifetimeController lifetime) : AppBase
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		base.OnFrameworkInitializationCompleted();

		PluginManager.LoadPlugins(serviceProvider);

		OpenConnectWindow(); // Controls the lifetime manually

		//if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		//{
		//	desktop.MainWindow = window;
		//}
		//else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
		//{
		//	singleViewPlatform.MainView = window;
		//}
	}

	private Adapter? _adapter;

	public override Adapter? Adapter => _adapter;

	public override Adapter EnsuredAdapter => Adapter
		?? throw new InvalidOperationException(SR.ExceptionAdapterNotSet);

	#region Connect Window

	public ConnectWindow? ConnectWindow { get; private set; }

	[MemberNotNull(nameof(ConnectWindow))]
	public void OpenConnectWindow()
	{
		if (ConnectWindow is not null)
		{
			throw new InvalidOperationException(SR.ExceptionConnectWindowAlreadyOpened);
		}

		_adapter = null;
		ConnectWindow = new ConnectWindow()
		{
			DataContext = new ConnectViewModel(),
		};
		ConnectWindow.Show();
	}

	public void OnConnectWindowClosed(Adapter? adapter)
	{
		if (ConnectWindow is null)
		{
			throw new InvalidOperationException(SR.ExceptionConnectWindowNotOpened);
		}

		_adapter = adapter;
		ConnectWindow = null;

		if (Adapter is null)
		{
			lifetime.Stop();
		}
		else
		{
			OpenMainPanelWindow();
		}
	}

	#endregion

	#region Main Panel Window

	public MainPanelWindow? MainPanelWindow { get; private set; }

	[MemberNotNull(nameof(MainPanelWindow))]
	public void OpenMainPanelWindow()
	{
		_ = EnsuredAdapter;
		if (MainPanelWindow is not null)
		{
			throw new InvalidOperationException(SR.ExceptionMainPanelWindowAlreadyOpened);
		}

		MainPanelWindow = new MainPanelWindow()
		{
			DataContext = new MainPanelViewModel(),
		};
		MainPanelWindow.Show();
	}

	public void OnMainPanelWindowClosed()
	{
		if (MainPanelWindow is null)
		{
			throw new InvalidOperationException(SR.ExceptionMainPanelWindowNotOpened);
		}

		MainPanelWindow = null;
	}

	[MemberNotNull(nameof(MainPanelWindow))]
	public void ShowMainPanelWindow()
	{
		if (MainPanelWindow is null)
		{
			OpenMainPanelWindow();
			return;
		}

		MainPanelWindow.Show();
		MainPanelWindow.Focus();
	}

	#endregion
}
