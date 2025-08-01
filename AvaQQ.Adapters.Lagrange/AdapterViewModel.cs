using Avalonia.Media.Imaging;
using ReactiveUI;

namespace AvaQQ.Adapters.Lagrange;

internal class AdapterViewModel : ReactiveObject
{
	private bool _isConnecting = false;

	public bool IsConnecting
	{
		get => _isConnecting;
		set
		{
			this.RaiseAndSetIfChanged(ref _isConnecting, value);
			ButtonConnectText = value ? string.Empty : SR.TextConnect;
			IsNotConnecting = !value;
		}
	}

	private bool _isNotConnecting = true;

	public bool IsNotConnecting
	{
		get => _isNotConnecting;
		set => this.RaiseAndSetIfChanged(ref _isNotConnecting, value);
	}

	private string _buttonConnectText = SR.TextConnect;

	public string ButtonConnectText
	{
		get => _buttonConnectText;
		set => this.RaiseAndSetIfChanged(ref _buttonConnectText, value);
	}

	private Bitmap? _qrCodeImage;

	public Bitmap? QrCodeImage
	{
		get => _qrCodeImage;
		set => this.RaiseAndSetIfChanged(ref _qrCodeImage, value);
	}
}
