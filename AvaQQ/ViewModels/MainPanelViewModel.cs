using Avalonia.Media.Imaging;
using AvaQQ.SDK;
using AvaQQ.Utils;
using ReactiveUI;
using System.Threading.Tasks;
using MainPanelConfig = AvaQQ.SDK.Configuration<AvaQQ.Configurations.MainPanelConfiguration>;

namespace AvaQQ.ViewModels;

internal class MainPanelViewModel : ViewModelBase
{
	private int _width = MainPanelConfig.Instance.Width;

	public int Width
	{
		get => _width;
		set
		{
			this.RaiseAndSetIfChanged(ref _width, value);
			MainPanelConfig.Instance.Width = value;
		}
	}

	private int _height = MainPanelConfig.Instance.Height;

	public int Height
	{
		get => _height;
		set
		{
			this.RaiseAndSetIfChanged(ref _height, value);
			MainPanelConfig.Instance.Height = value;
		}
	}

	#region Header

	private long _headerUin = 10000;

	public long HeaderUin
	{
		get => _headerUin;
		set
		{
			this.RaiseAndSetIfChanged(ref _headerUin, value);
			HeaderAvatar = value.GetAvatarImageAsync(100);
		}
	}

	private Task<Bitmap?> _headerAvatar = ((long)10000).GetAvatarImageAsync(40);

	public Task<Bitmap?> HeaderAvatar
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
