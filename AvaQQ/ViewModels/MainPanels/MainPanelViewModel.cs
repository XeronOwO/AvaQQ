using Avalonia.Media;
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

	#region Header

	private ulong _headerUin = 10000;

	public ulong HeaderUin
	{
		get => _headerUin;
		set => this.RaiseAndSetIfChanged(ref _headerUin, value);
	}

	private Task<IImage?> _headerAvatar = Task.FromResult<IImage?>(null);

	public Task<IImage?> HeaderAvatar
	{
		get => _headerAvatar;
		set => this.RaiseAndSetIfChanged(ref _headerAvatar, value);
	}

	private string _headerName = "User";

	public string HeaderName
	{
		get => _headerName;
		set => this.RaiseAndSetIfChanged(ref _headerName, value);
	}

	#endregion
}
