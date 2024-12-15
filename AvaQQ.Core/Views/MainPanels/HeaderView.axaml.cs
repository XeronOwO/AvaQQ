using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.Core.ViewModels.MainPanels;

namespace AvaQQ.Core.Views.MainPanels;

/// <summary>
/// ������ͼ
/// </summary>
public partial class HeaderView : UserControl
{
	/// <summary>
	/// ����������ͼ
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
