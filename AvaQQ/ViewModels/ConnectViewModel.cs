using AvaQQ.Resources;
using AvaQQ.SDK;
using ReactiveUI;

namespace AvaQQ.ViewModels;

public class ConnectViewModel : ViewModelBase
{
	public string TextConnect => SR.TextConnect;

	public string TextInputUrl => SR.TextInputUrl;

	public string TextInputAccessToken => SR.TextInputAccessToken;

	private bool _isConnecting;

	public bool IsConnecting
	{
		get => _isConnecting;
		set
		{
			this.RaiseAndSetIfChanged(ref _isConnecting, value);
			IsNotConnecting = !value;
		}
	}

	private bool _isNotConnecting = true;

	public bool IsNotConnecting
	{
		get => _isNotConnecting;
		set => this.RaiseAndSetIfChanged(ref _isNotConnecting, value);
	}
}
