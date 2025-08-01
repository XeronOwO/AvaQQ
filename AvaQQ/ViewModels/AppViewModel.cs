using AvaQQ.Resources;
using ReactiveUI;

namespace AvaQQ.ViewModels;

#pragma warning disable IDE0079
#pragma warning disable CA1822

internal class AppViewModel : ReactiveObject
{
	public string AppName => SR.AppName;

	public string TextExit => SR.MenuTextExit;
}
