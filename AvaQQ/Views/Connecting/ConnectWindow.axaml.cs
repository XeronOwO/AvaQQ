using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.Views;
using AvaQQ.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Configurations.ConnectConfiguration>;

namespace AvaQQ.Views.Connecting;

public partial class ConnectWindow : ConnectWindowBase
{
	public ConnectWindow(
		ConnectView connectView
		)
	{
		DataContext = new ConnectViewModel();
		InitializeComponent();
		gridConnectView.Children.Add(connectView);

		Closed += ConnectWindow_Closed;
	}

	public ConnectWindow() : this(
		DesignerServiceProviderHelper.Root.GetRequiredService<ConnectView>()
		)
	{
	}

	public override void BeginConnect()
	{
		if (DataContext is not ConnectViewModel model)
		{
			return;
		}

		model.IsConnecting = true;
	}

	private IAdapter? _adapter;

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

		_adapter = adapter;

		Close();
	}

	private void ConnectWindow_Closed(object? sender, EventArgs e)
	{
		Config.Save();

		AppBase.Current.OnConnected(_adapter);
	}
}
