using Avalonia.Controls;
using AvaQQ.SDK.Views;
using AvaQQ.ViewModels;
using AvaQQ.Views;
using System.Threading.Tasks;

namespace AvaQQ.Logging;

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
