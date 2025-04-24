using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using AvaQQ.Core.Views.Connecting;
using AvaQQ.SDK;
using Lagrange.Core.Common.Interface.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AvaQQ.Adapters.Lagrange;

public partial class AdapterSelectionView : UserControl
{
	private readonly ILogger<AdapterSelectionView> _logger;

	private readonly IServiceProvider _serviceProvider;

	private readonly IAppLifetimeController _controller;

	private readonly Lazy<ConnectWindow> _lazyConnectWindow;

	private ConnectWindow ConnectWindow => _lazyConnectWindow.Value;

	public AdapterSelectionView(IServiceProvider serviceProvider)
	{
		_logger = serviceProvider.GetRequiredService<ILogger<AdapterSelectionView>>();
		_serviceProvider = serviceProvider;
		_lazyConnectWindow = new(serviceProvider.GetRequiredService<ConnectWindow>);
		_controller = serviceProvider.GetRequiredService<IAppLifetimeController>();

		DataContext = new AdapterSelectionViewModel();
		InitializeComponent();
	}

	public AdapterSelectionView() : this(
		DesignerServiceProviderHelper.Root
		)
	{
	}

	private void ShowQrCode(byte[] qrCode)
	{
		Dispatcher.UIThread.Invoke(() =>
		{
			if (DataContext is not AdapterSelectionViewModel model)
			{
				return;
			}

			using var stream = new MemoryStream(qrCode);
			model.QrCodeImage = new Bitmap(stream);
		});
	}

	private async void ButtonConnect_Click(object? sender, RoutedEventArgs e)
	{
		if (DataContext is not AdapterSelectionViewModel model)
		{
			return;
		}

		ConnectWindow.BeginConnect();
		model.IsConnecting = true;

		Adapter? adapter = null;
		try
		{
			var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
			var context = await BotContextHelper.CreateBotContextAsync(configuration, _controller.CancellationToken);
			adapter = new Adapter(_serviceProvider, context);

			_logger.LogInformation("Trying login by easy.");
			if (await adapter.TryLoginByEasyAsync(_controller.CancellationToken))
			{
				_logger.LogInformation("Logged in by easy.");

				model.IsConnecting = false;
				ConnectWindow.EndConnect(adapter);
				return;
			}

			_logger.LogInformation("Trying login by QrCode.");
			if (await adapter.TryLoginByQrCodeAsync(ShowQrCode, _controller.CancellationToken))
			{
				_logger.LogInformation("Logged in by QrCode.");

				var keystorePath = configuration.GetValue("ConfigPath:Keystore", Path.Combine(Configuration.BaseDirectory, "keystore.json"))!;
				File.WriteAllText(keystorePath, JsonSerializer.Serialize(context.UpdateKeystore()));
				_logger.LogInformation("Keystore saved.");

				model.IsConnecting = false;
				ConnectWindow.EndConnect(adapter);
				return;
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to login.");
		}
		adapter?.Dispose();

		model.IsConnecting = false;
		model.QrCodeImage = null;
		ConnectWindow.EndConnect(null);
	}
}
