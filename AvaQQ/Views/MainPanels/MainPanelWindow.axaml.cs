using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.ViewModels;
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
		if (Application.Current is not App app
			|| DataContext is not MainPanelViewModel model
			|| Design.IsDesignMode)
		{
			return;
		}

		model.HeaderUin = app.EnsuredAdapter.Uin;
		model.HeaderName = await app.EnsuredAdapter.GetNicknameAsync();
	}

	private void MainPanelWindow_Closed(object? sender, EventArgs e)
	{
		if (Application.Current is not App app)
		{
			return;
		}

		MainPanelConfig.Save();
		app.MainPanelWindow = null;
	}
}
