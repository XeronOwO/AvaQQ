using AvaQQ.SDK;
using ReactiveUI;
using Config = AvaQQ.SDK.Configuration<Onebot11ForwardWebSocketAdapter.AdapterConfiguration>;

namespace Onebot11ForwardWebSocketAdapter;

internal class AdapterSelectionViewModel : ViewModelBase
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

	private string _url = Config.Instance.Url;

	public string Url
	{
		get => _url;
		set
		{
			this.RaiseAndSetIfChanged(ref _url, value);
			Config.Instance.Url = value;
		}
	}

	private string _accessToken = Config.Instance.AccessToken;

	public string AccessToken
	{
		get => _accessToken;
		set
		{
			this.RaiseAndSetIfChanged(ref _accessToken, value);
			Config.Instance.AccessToken = value;
		}
	}

	private string _buttonConnectText = SR.TextConnect;

	public string ButtonConnectText
	{
		get => _buttonConnectText;
		set => this.RaiseAndSetIfChanged(ref _buttonConnectText, value);
	}

	private string _textBlockErrorText = string.Empty;

	public string TextBlockErrorText
	{
		get => _textBlockErrorText;
		set => this.RaiseAndSetIfChanged(ref _textBlockErrorText, value);
	}
}
