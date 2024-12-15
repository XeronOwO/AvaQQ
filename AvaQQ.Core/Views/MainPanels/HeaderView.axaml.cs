using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.Core.ViewModels.MainPanels;

namespace AvaQQ.Core.Views.MainPanels;

/// <summary>
/// 顶部视图
/// </summary>
public partial class HeaderView : UserControl
{
	/// <summary>
	/// 创建顶部视图
	/// </summary>
	public HeaderView()
	{
		InitializeComponent();
	}

	private void ButtonUploadAvatar_Click(object? sender, RoutedEventArgs e)
	{

	}

	private void ButtonEditNickname_Click(object? sender, RoutedEventArgs e)
	{

	}

	private async void ButtonCopyUinToClipboard_Click(object? sender, RoutedEventArgs e)
	{
		if (DataContext is not MainPanelViewModel model
			|| VisualRoot is not Window window
			|| window.Clipboard is null)
		{
			return;
		}

		await window.Clipboard.SetTextAsync(model.HeaderUin.ToString());
	}

	private void ButtonMore_Click(object? sender, RoutedEventArgs e)
	{

	}
}
