using Avalonia.Media;
using AvaQQ.SDK;
using ReactiveUI;
using System.Threading.Tasks;

namespace AvaQQ.ViewModels.MainPanels;

internal class EntryViewModel : ViewModelBase
{
	private Task<IImage?> _icon = Task.FromResult<IImage?>(null);

	public Task<IImage?> Icon
	{
		get => _icon;
		set => this.RaiseAndSetIfChanged(ref _icon, value);
	}

	private string _title = string.Empty;

	public string Title
	{
		get => _title;
		set => this.RaiseAndSetIfChanged(ref _title, value);
	}

	private string _time = string.Empty;

	public string Time
	{
		get => _time;
		set => this.RaiseAndSetIfChanged(ref _time, value);
	}

	private string _content = string.Empty;

	public string Content
	{
		get => _content;
		set => this.RaiseAndSetIfChanged(ref _content, value);
	}

	private Task<IImage?> _contentIcon = Task.FromResult<IImage?>(null);

	public Task<IImage?> ContentIcon
	{
		get => _contentIcon;
		set
		{
			this.RaiseAndSetIfChanged(ref _contentIcon, value);
			IsContentIconVisible = _contentIcon != null;
		}
	}

	private bool _isContentIconVisible = false;

	public bool IsContentIconVisible
	{
		get => _isContentIconVisible;
		set => this.RaiseAndSetIfChanged(ref _isContentIconVisible, value);
	}

	private string _contentEmphasis = string.Empty;

	public string ContentEmphasis
	{
		get => _contentEmphasis;
		set
		{
			this.RaiseAndSetIfChanged(ref _contentEmphasis, value);
			IsContentEmphasisVisible = !string.IsNullOrEmpty(_contentEmphasis);
		}
	}

	private bool _isContentEmphasisVisible = false;

	public bool IsContentEmphasisVisible
	{
		get => _isContentEmphasisVisible;
		set => this.RaiseAndSetIfChanged(ref _isContentEmphasisVisible, value);
	}

	private int _unreadCount = 0;

	public int UnreadCount
	{
		get => _unreadCount;
		set
		{
			this.RaiseAndSetIfChanged(ref _unreadCount, value);
			IsUnreadCountVisible = _unreadCount > 0;
			UnreadCountText = _unreadCount > 99 ? "99+" : _unreadCount.ToString();
		}
	}

	private string _unreadCountText = string.Empty;

	public string UnreadCountText
	{
		get => _unreadCountText;
		set => this.RaiseAndSetIfChanged(ref _unreadCountText, value);
	}

	private bool _isUnreadCountVisible = false;

	public bool IsUnreadCountVisible
	{
		get => _isUnreadCountVisible;
		set => this.RaiseAndSetIfChanged(ref _isUnreadCountVisible, value);
	}
}
