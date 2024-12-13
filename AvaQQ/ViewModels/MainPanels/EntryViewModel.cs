using Avalonia.Media;
using AvaQQ.SDK;
using ReactiveUI;
using System.Threading.Tasks;

namespace AvaQQ.ViewModels.MainPanels;

internal class EntryViewModel : ViewModelBase
{
	public ulong Id { get; set; }

	public Task<IImage?> Icon
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = Task.FromResult<IImage?>(null);

	public string Title
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = string.Empty;

	public string Time
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = string.Empty;

	public Task<string> Content
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = Task.FromResult(string.Empty);

	public Task<IImage?> ContentIcon
	{
		get => field;
		set
		{
			this.RaiseAndSetIfChanged(ref field, value);
			IsContentIconVisible = field != null;
		}
	} = Task.FromResult<IImage?>(null);

	public bool IsContentIconVisible
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = false;

	public string ContentEmphasis
	{
		get => field;
		set
		{
			this.RaiseAndSetIfChanged(ref field, value);
			IsContentEmphasisVisible = !string.IsNullOrEmpty(field);
		}
	} = string.Empty;

	public bool IsContentEmphasisVisible
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = false;

	public int UnreadCount
	{
		get => field;
		set
		{
			this.RaiseAndSetIfChanged(ref field, value);
			IsUnreadCountVisible = field > 0;
			UnreadCountText = field > 99 ? "99+" : field.ToString();
		}
	} = 0;

	public string UnreadCountText
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = string.Empty;

	public bool IsUnreadCountVisible
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = false;
}
