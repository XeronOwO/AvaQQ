using Avalonia.Media.Imaging;
using AvaQQ.SDK;
using ReactiveUI;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.MainPanelConfiguration>;

namespace AvaQQ.Core.ViewModels.MainPanels;

/// <summary>
/// 主面板视图模型
/// </summary>
public class MainPanelViewModel : ViewModelBase
{
	private int _width = Config.Instance.Width;

	/// <summary>
	/// 宽度
	/// </summary>
	public int Width
	{
		get => _width;
		set
		{
			this.RaiseAndSetIfChanged(ref _width, value);
			Config.Instance.Width = value;
		}
	}

	private int _height = Config.Instance.Height;

	/// <summary>
	/// 高度
	/// </summary>
	public int Height
	{
		get => _height;
		set
		{
			this.RaiseAndSetIfChanged(ref _height, value);
			Config.Instance.Height = value;
		}
	}

	private ulong _headerUin = 10000;

	/// <summary>
	/// 顶部 QQ 号
	/// </summary>
	public ulong HeaderUin
	{
		get => _headerUin;
		set => this.RaiseAndSetIfChanged(ref _headerUin, value);
	}

	private Bitmap? _headerAvatar = null;

	/// <summary>
	/// 顶部头像
	/// </summary>
	public Bitmap? HeaderAvatar
	{
		get => _headerAvatar;
		set => this.RaiseAndSetIfChanged(ref _headerAvatar, value);
	}

	private string _headerName = "User";

	/// <summary>
	/// 顶部名称
	/// </summary>
	public string HeaderName
	{
		get => _headerName;
		set => this.RaiseAndSetIfChanged(ref _headerName, value);
	}
}
