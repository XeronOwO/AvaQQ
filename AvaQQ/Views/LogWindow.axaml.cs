using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.ViewModels;

namespace AvaQQ.Views;

public partial class LogWindow : Window
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
