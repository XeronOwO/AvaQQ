using Avalonia.Media;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace AvaQQ.ViewModels.Main;

public class EntryViewModel : ReactiveObject
{
	public Bitmap? Image
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	}

	public string Title
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = nameof(Title);

	public DateTimeOffset Time
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = DateTimeOffset.MinValue;

	public string Preview
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = nameof(Preview);

	public SolidColorBrush UnreadBackgroundBrush
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = new(Colors.Red);

	public uint UnreadCount
	{
		get => field;
		set
		{
			this.RaiseAndSetIfChanged(ref field, value);

			if (field == 0)
			{
				UnreadVisible = false;
				return;
			}
			UnreadCountText = field < 100 ? field.ToString() : "99+";
		}
	} = 100;

	public string UnreadCountText
	{
		get => field;
		private set => this.RaiseAndSetIfChanged(ref field, value);
	} = "99+";

	public bool UnreadVisible
	{
		get => field;
		private set => this.RaiseAndSetIfChanged(ref field, value);
	} = true;
}
