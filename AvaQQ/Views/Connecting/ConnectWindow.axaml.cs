using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.Databases;
using AvaQQ.SDK.ViewModels;
using AvaQQ.ViewModels;
using AvaQQ.ViewModels.MainPanels;
using AvaQQ.Views.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using System;
using ConnectConfig = AvaQQ.SDK.Configuration<AvaQQ.Configurations.ConnectConfiguration>;

namespace AvaQQ.Views.Connecting;

public partial class ConnectWindow : ConnectWindowBase
{
	public ConnectWindow()
	{
		InitializeComponent();

		Closed += ConnectWindow_Closed;
	}

	private void ConnectWindow_Closed(object? sender, EventArgs e)
	{
		var app = AppBase.Current;

		ConnectConfig.Save();
		if (app.Adapter is null)
		{
			app.Lifetime.Stop();
		}
		app.ConnectWindow = null;
	}

	public override void BeginConnect()
	{
		if (DataContext is not ConnectViewModel model)
		{
			return;
		}

		model.IsConnecting = true;
	}

	public override void EndConnect(IAdapter? adapter)
	{
		if (DataContext is not ConnectViewModel model)
		{
			return;
		}

		model.IsConnecting = false;

		if (adapter is null)
		{
			return;
		}

		var app = AppBase.Current;
		app.Adapter = adapter;
		Close();

		app.MainPanelWindow = new MainPanelWindow()
		{
			DataContext = new MainPanelViewModel(),
		};
		app.MainPanelWindow.Show();

		AppBase.Current.ServiceProvider.GetRequiredService<GroupMessageDatabase>()
			.Initialize();
	}
}
