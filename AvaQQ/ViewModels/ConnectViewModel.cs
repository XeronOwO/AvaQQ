using AvaQQ.Resources;
using ReactiveUI;
using ConnectConfig = AvaQQ.SDK.Configuration<AvaQQ.Configurations.ConnectConfiguration>;

namespace AvaQQ.ViewModels;

public class ConnectViewModel : ViewModelBase
{
	public string TextConnect => SR.TextConnect;

	public string TextInputUrl => SR.TextInputUrl;

	public string TextInputAccessToken => SR.TextInputAccessToken;

	#region Common

	private bool _isConnecting;

	public bool IsConnecting
	{
		get => _isConnecting;
		set
		{
			this.RaiseAndSetIfChanged(ref _isConnecting, value);
			Onebot11ForwardWebSocketButtonConnectText = value ? string.Empty : SR.TextConnect;
			IsNotConnecting = !value;
		}
	}

	private bool _isNotConnecting = true;

	public bool IsNotConnecting
	{
		get => _isNotConnecting;
		set => this.RaiseAndSetIfChanged(ref _isNotConnecting, value);
	}

	#endregion

	#region Onebot 11 Forward WebSocket

	private bool _isOnebot11ForwardWebSocketAdapter;

	public bool IsOnebot11ForwardWebSocketAdapter
	{
		get => _isOnebot11ForwardWebSocketAdapter;
		set => this.RaiseAndSetIfChanged(ref _isOnebot11ForwardWebSocketAdapter, value);
	}

	private string _onebot11ForwardWebSocketUrl = ConnectConfig.Instance.Onebot11ForwardWebSocket.Url;

	public string Onebot11ForwardWebSocketUrl
	{
		get => _onebot11ForwardWebSocketUrl;
		set
		{
			this.RaiseAndSetIfChanged(ref _onebot11ForwardWebSocketUrl, value);
			ConnectConfig.Instance.Onebot11ForwardWebSocket.Url = value;
		}
	}

	private string _onebot11ForwardWebSocketAccessToken = ConnectConfig.Instance.Onebot11ForwardWebSocket.AccessToken;

	public string Onebot11ForwardWebSocketAccessToken
	{
		get => _onebot11ForwardWebSocketAccessToken;
		set
		{
			this.RaiseAndSetIfChanged(ref _onebot11ForwardWebSocketAccessToken, value);
			ConnectConfig.Instance.Onebot11ForwardWebSocket.AccessToken = value;
		}
	}

	private string _onebot11ForwardWebSocketButtonConnectText = SR.TextConnect;

	public string Onebot11ForwardWebSocketButtonConnectText
	{
		get => _onebot11ForwardWebSocketButtonConnectText;
		set => this.RaiseAndSetIfChanged(ref _onebot11ForwardWebSocketButtonConnectText, value);
	}

	private string _onebot11ForwardWebSocketTextBlockErrorText = string.Empty;

	public string Onebot11ForwardWebSocketTextBlockErrorText
	{
		get => _onebot11ForwardWebSocketTextBlockErrorText;
		set => this.RaiseAndSetIfChanged(ref _onebot11ForwardWebSocketTextBlockErrorText, value);
	}

	#endregion
}
