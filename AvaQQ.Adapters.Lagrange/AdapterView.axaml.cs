using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using AvaQQ.SDK;
using Lagrange.Core.Common.Interface.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AvaQQ.Adapters.Lagrange;

internal partial class AdapterView : UserControl
{
	private readonly ILogger<AdapterView> _logger;

	private readonly IServiceProvider _serviceProvider;

	private readonly IAppLifetime _lifetime;

	private readonly IConnectWindowProvider _connectWindowProvider;

	public AdapterViewModel ViewModel
	{
		get => (AdapterViewModel)(DataContext ?? throw new ArgumentNullException(nameof(ViewModel)));
		set => DataContext = value;
	}

	public AdapterView()
	{
		var serviceProvider = AppBase.Current.Services;

		_logger = serviceProvider.GetRequiredService<ILogger<AdapterView>>();
		_serviceProvider = serviceProvider;
		_connectWindowProvider = serviceProvider.GetRequiredService<IConnectWindowProvider>();
		_lifetime = serviceProvider.GetRequiredService<IAppLifetime>();

		ViewModel = new AdapterViewModel();
		InitializeComponent();
	}

	private void ShowQrCode(byte[] qrCode)
	{
		Dispatcher.UIThread.Invoke(() =>
		{
			using var stream = new MemoryStream(qrCode);
			ViewModel.QrCodeImage = new Bitmap(stream);
		});
	}

	private async void ButtonConnect_Click(object? sender, RoutedEventArgs e)
	{
		if (_connectWindowProvider.Window is not ConnectWindowBase window)
		{
			return;
		}

		window.BeginConnect();
		ViewModel.IsConnecting = true;

		Adapter? adapter = null;
		try
		{
			var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
			var context = await BotContextCreator.CreateBotContextAsync(configuration, _lifetime.Token);
			adapter = new Adapter(
				_serviceProvider.GetRequiredService<ILogger<Adapter>>(),
				_serviceProvider.GetRequiredService<ILoggerProvider>(),
				context);

			_logger.LogInformation("Trying login by easy");
			if (await adapter.TryLoginByEasyAsync(_lifetime.Token))
			{
				_logger.LogInformation("Logged in by easy");

				ViewModel.IsConnecting = false;
				window.EndConnect(adapter);
				return;
			}

			_logger.LogInformation("Trying login by QrCode");
			if (await adapter.TryLoginByQrCodeAsync(ShowQrCode, _lifetime.Token))
			{
				_logger.LogInformation("Logged in by QrCode");

				var keystorePath = configuration.GetValue("ConfigPath:Keystore", Path.Combine(Configuration.BaseDirectory, "keystore.json"))!;
				File.WriteAllText(keystorePath, JsonSerializer.Serialize(context.UpdateKeystore()));
				_logger.LogInformation("Keystore saved");

				ViewModel.IsConnecting = false;
				window.EndConnect(adapter);
				return;
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to login");
		}
		adapter?.Dispose();

		ViewModel.IsConnecting = false;
		ViewModel.QrCodeImage = null;
		window.EndConnect(null);
	}
}
