using AvaQQ.Resources;
using AvaQQ.SDK;
using ReactiveUI;

namespace AvaQQ.ViewModels;

public class LogViewModel : ViewModelBase
{
	public string TextLogs => SR.TextLogs;

	public string TextCopy => SR.TextCopy;

	private string _content = string.Empty;

	public string Content
	{
		get => _content;
		set => this.RaiseAndSetIfChanged(ref _content, value);
	}
}
