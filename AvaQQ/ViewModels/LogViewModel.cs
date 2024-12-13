using AvaQQ.Resources;
using AvaQQ.SDK;
using ReactiveUI;

namespace AvaQQ.ViewModels;

public class LogViewModel : ViewModelBase
{
	public string TextLogs => SR.TextLogs;

	public string TextCopy => SR.TextCopy;

	public string Content
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = string.Empty;
}
