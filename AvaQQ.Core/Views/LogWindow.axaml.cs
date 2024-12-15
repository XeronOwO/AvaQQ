using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.Core.ViewModels;

namespace AvaQQ.Core.Views;

internal partial class LogWindow : Window
{
	public LogWindow()
	{
		InitializeComponent();
	}

	private async void ButtonCopyLog_Click(object? sender, RoutedEventArgs e)
	{
		if (Clipboard is not null
			&& DataContext is LogViewModel model)
		{
			await Clipboard.SetTextAsync(model.Content);
		}
	}
}
