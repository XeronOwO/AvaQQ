using AvaQQ.SDK;
using ReactiveUI;
using Config = AvaQQ.SDK.Configuration<Onebot11ForwardWebSocketAdapter.AdapterConfiguration>;

namespace Onebot11ForwardWebSocketAdapter;

internal class AdapterSelectionViewModel : ViewModelBase
{
	public string TextConnect => SR.TextConnect;

	public string TextInputUrl => SR.TextInputUrl;

	public string TextInputAccessToken => SR.TextInputAccessToken;

	public bool IsConnecting
	{
		get => field;
		set
		{
			this.RaiseAndSetIfChanged(ref field, value);
			ButtonConnectText = value ? string.Empty : SR.TextConnect;
			IsNotConnecting = !value;
		}
	} = false;

	public bool IsNotConnecting
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = true;

	public string Url
	{
		get => field;
		set
		{
			this.RaiseAndSetIfChanged(ref field, value);
			Config.Instance.Url = value;
		}
	} = Config.Instance.Url;

	public string AccessToken
	{
		get => field;
		set
		{
			this.RaiseAndSetIfChanged(ref field, value);
			Config.Instance.AccessToken = value;
		}
	} = Config.Instance.AccessToken;

	public string ButtonConnectText
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = SR.TextConnect;

	public string TextBlockErrorText
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = string.Empty;
}
