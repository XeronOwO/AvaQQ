using Avalonia.Media.Imaging;
using AvaQQ.SDK;
using ReactiveUI;
using System.Threading.Tasks;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Configurations.MainPanelConfiguration>;

namespace AvaQQ.ViewModels.MainPanels;

internal class MainPanelViewModel : ViewModelBase
{
	private int _width = Config.Instance.Width;

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

	public ulong HeaderUin
	{
		get => _headerUin;
		set => this.RaiseAndSetIfChanged(ref _headerUin, value);
	}

	private Task<Bitmap?> _headerAvatar = Task.FromResult<Bitmap?>(null);

	public Task<Bitmap?> HeaderAvatar
	{
		get => _headerAvatar;
		set => this.RaiseAndSetIfChanged(ref _headerAvatar, value);
	}

	private Task<string> _headerName = Task.FromResult("User");

	public Task<string> HeaderName
	{
		get => _headerName;
		set => this.RaiseAndSetIfChanged(ref _headerName, value);
	}
}
