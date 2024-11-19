using Avalonia;
using Avalonia.Controls;
using AvaQQ.SDK;
using System;
using ConnectConfig = AvaQQ.SDK.Configuration<AvaQQ.Configurations.ConnectConfiguration>;

namespace AvaQQ.Views.Connecting;

public partial class ConnectWindow : Window
{
	public ConnectWindow()
	{
		InitializeComponent();

		Closed += ConnectWindow_Closed;
	}

	public Adapter? Adapter { get; set; }

	private void ConnectWindow_Closed(object? sender, EventArgs e)
	{
		if (Application.Current is not App app)
		{
			return;
		}

		app.OnConnectWindowClosed(Adapter);
		ConnectConfig.Save();
	}
}
