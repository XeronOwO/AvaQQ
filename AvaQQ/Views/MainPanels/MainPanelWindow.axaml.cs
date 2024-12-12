using Avalonia.Controls;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.ViewModels.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Configurations.MainPanelConfiguration>;

namespace AvaQQ.Views.MainPanels;

public partial class MainPanelWindow : Window
{
	public MainPanelWindow(
		IAvatarCache avatarCache,
		MainPanelView view
		)
	{
		var app = AppBase.Current;
		var model = new MainPanelViewModel();
		if (app.Adapter is { } adapter)
		{
			model.HeaderUin = adapter.Uin;
			model.HeaderAvatar = avatarCache.GetUserAvatarAsync(adapter.Uin, 40);
			model.HeaderName = adapter.GetNicknameAsync();
		}
		DataContext = model;
		InitializeComponent();
		gridMainPanelView.Children.Add(view);

		Closed += (_, _) =>
		{
			Config.Save();
		};
	}

	public MainPanelWindow() : this(
		DesignerServiceProviderHelper.Root.GetRequiredService<IAvatarCache>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<MainPanelView>()
		)
	{
	}
}
