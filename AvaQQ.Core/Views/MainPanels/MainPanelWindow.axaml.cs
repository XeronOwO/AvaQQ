using Avalonia.Controls;
using AvaQQ.Core.Adapters;
using AvaQQ.Core.Caches;
using AvaQQ.Core.ViewModels.MainPanels;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.MainPanelConfiguration>;

namespace AvaQQ.Core.Views.MainPanels;

/// <summary>
/// 主面板窗口
/// </summary>
public partial class MainPanelWindow : Window
{
	/// <summary>
	/// 创建主面板窗口
	/// </summary>
	public MainPanelWindow(
		IAdapterProvider adapterProvider,
		IAvatarCache avatarCache,
		MainPanelView view
		)
	{
		var model = new MainPanelViewModel();
		if (adapterProvider.Adapter is { } adapter)
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

	/// <summary>
	/// 创建主面板窗口
	/// </summary>
	public MainPanelWindow() : this(
		DesignerServiceProviderHelper.Root.GetRequiredService<IAdapterProvider>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<IAvatarCache>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<MainPanelView>()
		)
	{
	}
}
