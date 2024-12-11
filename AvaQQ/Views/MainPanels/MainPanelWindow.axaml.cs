using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.ViewModels.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using System;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Configurations.MainPanelConfiguration>;

namespace AvaQQ.Views.MainPanels;

public partial class MainPanelWindow : Window
{
	private readonly IAvatarCache _avatarCache;

	public MainPanelWindow(IAvatarCache avatarCache)
	{
		_avatarCache = avatarCache;

		InitializeComponent();

		Loaded += MainPanelWindow_Loaded;
		Closed += MainPanelWindow_Closed;
	}

	public MainPanelWindow()
		: this(DesignerServiceProviderHelper.Root.GetRequiredService<IAvatarCache>())
	{
	}

	private async void MainPanelWindow_Loaded(object? sender, RoutedEventArgs e)
	{
		var app = AppBase.Current;
		if (DataContext is not MainPanelViewModel model
			|| Design.IsDesignMode
			|| app.Adapter is not { } adapter)
		{
			return;
		}

		model.HeaderUin = adapter.Uin;
		model.HeaderAvatar = _avatarCache.GetUserAvatarAsync(adapter.Uin, 40);
		model.HeaderName = await adapter.GetNicknameAsync();
	}

	private void MainPanelWindow_Closed(object? sender, EventArgs e)
	{
		Config.Save();
		AppBase.Current.MainPanelWindow = null;
	}
}
