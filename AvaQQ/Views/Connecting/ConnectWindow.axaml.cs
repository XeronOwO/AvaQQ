using Avalonia;
using Avalonia.Controls;
using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.ViewModels;
using AvaQQ.ViewModels;
using AvaQQ.ViewModels.MainPanels;
using AvaQQ.Views.MainPanels;
using System;
using ConnectConfig = AvaQQ.SDK.Configuration<AvaQQ.Configurations.ConnectConfiguration>;

namespace AvaQQ.Views.Connecting;

public partial class ConnectWindow : Window, IConnectWindow
{
	public ConnectWindow()
	{
		InitializeComponent();

		Closed += ConnectWindow_Closed;
	}

	private void ConnectWindow_Closed(object? sender, EventArgs e)
	{
		if (Application.Current is not App app)
		{
			return;
		}

		ConnectConfig.Save();
		if (app.Adapter is null)
		{
			app.Lifetime.Stop();
		}
		app.ConnectWindow = null;
	}

	public void BeginConnect()
	{
		if (DataContext is not ConnectViewModel model)
		{
			return;
		}

		model.IsConnecting = true;
	}

	public void EndConnect(IAdapter? adapter)
	{
		if (DataContext is not ConnectViewModel model)
		{
			return;
		}

		model.IsConnecting = false;

		if (Application.Current is not App app
			|| adapter is null)
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
