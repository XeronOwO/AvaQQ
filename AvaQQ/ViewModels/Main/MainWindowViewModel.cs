using ReactiveUI;

namespace AvaQQ.ViewModels.Main;

public class MainWindowViewModel : ReactiveObject
{
	public string Title
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = nameof(Title);
}
