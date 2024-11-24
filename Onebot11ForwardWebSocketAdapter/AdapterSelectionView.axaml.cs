using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.SDK;
using AvaQQ.SDK.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Config = AvaQQ.SDK.Configuration<Onebot11ForwardWebSocketAdapter.AdapterConfiguration>;

namespace Onebot11ForwardWebSocketAdapter;

public partial class AdapterSelectionView : UserControl
{
	public AdapterSelectionView()
	{
		InitializeComponent();

		Unloaded += AdapterSelectionView_Unloaded;
	}

	private void AdapterSelectionView_Unloaded(object? sender, RoutedEventArgs e)
	{
		Config.Save();
	}

	private async void ButtonConnect_Click(object? sender, RoutedEventArgs e)
	{
		if (DataContext is not AdapterSelectionViewModel model
			|| VisualRoot is not Window window
			|| VisualRoot is not ConnectWindowBase connect)
		{
			return;
		}

		var app = AppBase.Current;

		connect.BeginConnect();
		model.IsConnecting = true;
		model.TextBlockErrorText = string.Empty;

		var adapter = new Adapter(app.ServiceProvider, model.Url, model.AccessToken);
		var (success, log) = await adapter.TryConnectAsync(Constants.ConnectionSpan);

		if (!success)
		{
			adapter.Dispose();
			model.IsConnecting = false;
			model.TextBlockErrorText = SR.TextConnectFailed;
			connect.EndConnect(null);

			await app.ServiceProvider.GetRequiredService<ILogWindowProvider>().ShowDialog(
				window,
				log.ToString()
			);

			return;
		}

		model.IsConnecting = false;
		connect.EndConnect(adapter);
	}
}
