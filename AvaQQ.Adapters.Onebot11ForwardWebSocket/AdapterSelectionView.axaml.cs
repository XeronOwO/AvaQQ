using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.Core.Logging;
using AvaQQ.Core.Views.Connecting;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Adapters.Onebot11ForwardWebSocket.AdapterConfiguration>;

namespace AvaQQ.Adapters.Onebot11ForwardWebSocket;

public partial class AdapterSelectionView : UserControl
{
	private readonly IServiceProvider _serviceProvider;

	private readonly ILogWindowProvider _logWindowProvider;

	private readonly Lazy<ConnectWindow> _lazyConnectWindow;

	private ConnectWindow ConnectWindow => _lazyConnectWindow.Value;

	public AdapterSelectionView(
		IServiceProvider serviceProvider,
		ILogWindowProvider logWindowProvider
		)
	{
		_serviceProvider = serviceProvider;
		_logWindowProvider = logWindowProvider;
		_lazyConnectWindow = new(serviceProvider.GetRequiredService<ConnectWindow>);

		DataContext = new AdapterSelectionViewModel();
		InitializeComponent();

		Unloaded += AdapterSelectionView_Unloaded;
	}

	public AdapterSelectionView() : this(
		DesignerServiceProviderHelper.Root,
		DesignerServiceProviderHelper.Root.GetRequiredService<ILogWindowProvider>()
		)
	{
	}

	private void AdapterSelectionView_Unloaded(object? sender, RoutedEventArgs e)
	{
		Config.Save();
	}

	private async void ButtonConnect_Click(object? sender, RoutedEventArgs e)
	{
		if (DataContext is not AdapterSelectionViewModel model)
		{
			return;
		}

		ConnectWindow.BeginConnect();
		model.IsConnecting = true;
		model.TextBlockErrorText = string.Empty;

		var adapter = new Adapter(_serviceProvider, model.Url, model.AccessToken);
		var (success, log) = await adapter.TryConnectAsync(Config.Instance.ConnectTimeout);

		if (!success)
		{
			adapter.Dispose();
			model.IsConnecting = false;
			model.TextBlockErrorText = SR.TextConnectFailed;
			ConnectWindow.EndConnect(null);

			await _logWindowProvider.ShowDialog(
				ConnectWindow,
				log.ToString()
			);

			return;
		}

		model.IsConnecting = false;
		ConnectWindow.EndConnect(adapter);
	}
}
