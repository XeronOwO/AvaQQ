using Avalonia.Controls;
using AvaQQ.Core.ViewModels;
using AvaQQ.Core.Views;

namespace AvaQQ.Core.Logging;

internal class LogWindowProvider : ILogWindowProvider
{
	public void Show(string log)
	{
		var logWindow = new LogWindow()
		{
			DataContext = new LogViewModel()
			{
				Content = log,
			},
		};
		logWindow.Show();
	}

	public Task ShowDialog(Window window, string log)
	{
		var logWindow = new LogWindow()
		{
			DataContext = new LogViewModel()
			{
				Content = log,
			},
		};
		return logWindow.ShowDialog(window);
	}
}
