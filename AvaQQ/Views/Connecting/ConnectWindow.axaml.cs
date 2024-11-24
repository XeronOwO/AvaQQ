using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.ViewModels;
using AvaQQ.ViewModels;
using AvaQQ.ViewModels.MainPanels;
using AvaQQ.Views.MainPanels;
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

		var app = AppBase.Current;
		if (adapter is null)
		{
			return;
		}

		app.Adapter = adapter;
		Close();

		app.MainPanelWindow = new MainPanelWindow()
		{
			DataContext = new MainPanelViewModel(),
		};
		app.MainPanelWindow.Show();
	}
}
