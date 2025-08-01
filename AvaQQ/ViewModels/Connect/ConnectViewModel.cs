using AvaQQ.Resources;
using ReactiveUI;

namespace AvaQQ.ViewModels.Connect;

#pragma warning disable IDE0079
#pragma warning disable CA1822

public class ConnectViewModel : ReactiveObject
{
	public string AppName => SR.AppName;

	public string TextConnect => SR.TextConnect;

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
		private set => this.RaiseAndSetIfChanged(ref field, value);
	} = true;
}
