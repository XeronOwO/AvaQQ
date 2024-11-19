using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.Adapters;
using AvaQQ.Resources;
using AvaQQ.SDK;
using AvaQQ.ViewModels;

namespace AvaQQ.Views.Connecting;

public partial class Onebot11ForwardWebSocketAdapterOptionsView : UserControl
{
	public Onebot11ForwardWebSocketAdapterOptionsView()
	{
		InitializeComponent();
	}

	private async void ButtonConnect_Click(object? sender, RoutedEventArgs e)
	{
		if (DataContext is not ConnectViewModel model
			|| VisualRoot is not ConnectWindow window)
		{
			return;
		}

		model.IsConnecting = true;
		model.Onebot11ForwardWebSocketTextBlockErrorText = string.Empty;

		var adapter = new Onebot11ForwardWebSocketAdapter(
			model.Onebot11ForwardWebSocketUrl,
			model.Onebot11ForwardWebSocketAccessToken
		);

		if (!await adapter.ConnectAsync(Constants.ConnectionSpan))
		{
			adapter.Dispose();
			model.IsConnecting = false;
			model.Onebot11ForwardWebSocketTextBlockErrorText = SR.TextConnectFailed;

			var logWindow = new LogWindow()
			{
				DataContext = new LogViewModel()
				{
					Logs = adapter.Logs,
				},
			};
			await logWindow.ShowDialog(window);

			return;
		}

		model.IsConnecting = false;
		window.Adapter = adapter;
		window.Close();
	}
}
