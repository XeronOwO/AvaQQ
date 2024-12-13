using AvaQQ.Resources;
using AvaQQ.SDK;
using ReactiveUI;

namespace AvaQQ.ViewModels;

public class ConnectViewModel : ViewModelBase
{
	public string AppName => SR.AppName;

	public string TextConnect => SR.TextConnect;

	public string TextInputUrl => SR.TextInputUrl;

	public string TextInputAccessToken => SR.TextInputAccessToken;

	public bool IsConnecting
	{
		get => field;
		set
		{
			this.RaiseAndSetIfChanged(ref field, value);
			IsNotConnecting = !value;
		}
	} = false;

	public bool IsNotConnecting
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = true;
}
