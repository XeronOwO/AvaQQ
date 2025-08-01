using AvaQQ.Resources;
using ReactiveUI;

namespace AvaQQ.ViewModels.Connect;

#pragma warning disable IDE0079
#pragma warning disable CA1822

public class ConnectWindowViewModel : ReactiveObject
{
	public string Title => SR.TextConnect;
}
