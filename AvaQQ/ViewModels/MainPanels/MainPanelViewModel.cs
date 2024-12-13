using Avalonia.Media;
using AvaQQ.SDK;
using ReactiveUI;
using System.Threading.Tasks;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Configurations.MainPanelConfiguration>;

namespace AvaQQ.ViewModels.MainPanels;

internal class MainPanelViewModel : ViewModelBase
{
	public int Width
	{
		get => field;
		set
		{
			this.RaiseAndSetIfChanged(ref field, value);
			Config.Instance.Width = value;
		}
	} = Config.Instance.Width;

	public int Height
	{
		get => field;
		set
		{
			this.RaiseAndSetIfChanged(ref field, value);
			Config.Instance.Height = value;
		}
	} = Config.Instance.Height;

	public ulong HeaderUin
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = 10000;

	public Task<IImage?> HeaderAvatar
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = Task.FromResult<IImage?>(null);

	public Task<string> HeaderName
	{
		get => field;
		set => this.RaiseAndSetIfChanged(ref field, value);
	} = Task.FromResult("User");
}
