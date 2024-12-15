using Avalonia.Media.Imaging;
using AvaQQ.SDK;
using ReactiveUI;

namespace AvaQQ.Core.ViewModels.MainPanels;

/// <summary>
/// 条目视图模型
/// </summary>
public class EntryViewModel : ViewModelBase
{
	public ulong Id { get; set; }

	private Task<Bitmap?> _icon = Task.FromResult<Bitmap?>(null);

	/// <summary>
	/// 图标
	/// </summary>
	public Task<Bitmap?> Icon
	{
		get => _icon;
		set => this.RaiseAndSetIfChanged(ref _icon, value);
	}

	private string _title = string.Empty;

	/// <summary>
	/// 标题
	/// </summary>
	public string Title
	{
		get => _title;
		set => this.RaiseAndSetIfChanged(ref _title, value);
	}

	private string _time = string.Empty;

	/// <summary>
	/// 时间
	/// </summary>
	public string Time
	{
		get => _time;
		set => this.RaiseAndSetIfChanged(ref _time, value);
	}

	private Task<string> _content = Task.FromResult(string.Empty);

	/// <summary>
	/// 内容
	/// </summary>
	public Task<string> Content
	{
		get => _content;
		set => this.RaiseAndSetIfChanged(ref _content, value);
	}

	private Task<Bitmap?> _contentIcon = Task.FromResult<Bitmap?>(null);

	/// <summary>
	/// 内容图标
	/// </summary>
	public Task<Bitmap?> ContentIcon
	{
		get => _contentIcon;
		set
		{
			this.RaiseAndSetIfChanged(ref _contentIcon, value);
			IsContentIconVisible = _contentIcon != null;
		}
	}

	private bool _isContentIconVisible = false;

	/// <summary>
	/// 是否显示内容图标
	/// </summary>
	public bool IsContentIconVisible
	{
		get => _isContentIconVisible;
		private set => this.RaiseAndSetIfChanged(ref _isContentIconVisible, value);
	}

	private string _contentEmphasis = string.Empty;

	/// <summary>
	/// 内容强调
	/// </summary>
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

	/// <summary>
	/// 是否显示内容强调
	/// </summary>
	public bool IsContentEmphasisVisible
	{
		get => _isContentEmphasisVisible;
		private set => this.RaiseAndSetIfChanged(ref _isContentEmphasisVisible, value);
	}

	private int _unreadCount = 0;

	/// <summary>
	/// 未读数
	/// </summary>
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

	/// <summary>
	/// 未读数文本
	/// </summary>
	public string UnreadCountText
	{
		get => _unreadCountText;
		private set => this.RaiseAndSetIfChanged(ref _unreadCountText, value);
	}

	private bool _isUnreadCountVisible = false;

	/// <summary>
	/// 是否显示未读数
	/// </summary>
	public bool IsUnreadCountVisible
	{
		get => _isUnreadCountVisible;
		private set => this.RaiseAndSetIfChanged(ref _isUnreadCountVisible, value);
	}
}
