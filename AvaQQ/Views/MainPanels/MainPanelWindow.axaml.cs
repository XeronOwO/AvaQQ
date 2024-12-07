using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.ViewModels.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using System;
using MainPanelConfig = AvaQQ.SDK.Configuration<AvaQQ.Configurations.MainPanelConfiguration>;

namespace AvaQQ.Views.MainPanels;

public partial class MainPanelWindow : Window
{
	public MainPanelWindow()
	{
		InitializeComponent();

		Loaded += MainPanelWindow_Loaded;
		Closed += MainPanelWindow_Closed;
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
		model.HeaderAvatar = app.ServiceProvider.GetRequiredService<IAvatarCache>()
			.GetUserAvatarAsync(adapter.Uin, 40);
		model.HeaderName = await adapter.GetNicknameAsync();
	}

	private void MainPanelWindow_Closed(object? sender, EventArgs e)
	{
		MainPanelConfig.Save();
		AppBase.Current.MainPanelWindow = null;
	}
}
