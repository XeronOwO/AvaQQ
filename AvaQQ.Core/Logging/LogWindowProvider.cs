using Avalonia.Controls;
using AvaQQ.Core.ViewModels;
using AvaQQ.Core.Views;
using Microsoft.Extensions.Logging;

namespace AvaQQ.Core.Logging;

internal class LogWindowProvider(ILogger<LogWindowProvider> logger) : ILogWindowProvider
{
	public void Show(string log)
		=> CreateDialog(log).Show();

	public Task ShowDialog(Window window, string log)
		=> CreateDialog(log).ShowDialog(window);

	private LogWindow CreateDialog(string log)
	{
		logger.LogInformation("Creating {Window}.", nameof(LogWindow));

		return new()
		{
			DataContext = new LogViewModel()
			{
				Content = log,
			},
		};
	}
}
