using Avalonia.Media.Imaging;
using ReactiveUI;

namespace AvaQQ.ViewModels.Main;

public class MainViewModel : ReactiveObject
{
	public Bitmap? Avatar
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	}

	public string Nickname
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = nameof(Nickname);
}
