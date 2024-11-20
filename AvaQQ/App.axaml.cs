using Avalonia.Markup.Xaml;
using AvaQQ.Plugins;
using AvaQQ.Resources;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.ViewModels;
using AvaQQ.Views.Connecting;
using AvaQQ.Views.MainPanels;
using System;

namespace AvaQQ;

public partial class App : AppBase
{
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
	}

	public override IAdapter? Adapter { get; set; }

	public override IAdapter EnsuredAdapter => Adapter
		?? throw new InvalidOperationException(SR.ExceptionAdapterNotSet);

	public ConnectWindow? ConnectWindow { get; set; }

	public MainPanelWindow? MainPanelWindow { get; set; }
}
