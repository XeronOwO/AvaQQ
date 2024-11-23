using Avalonia.Controls;
using AvaQQ.Resources;
using AvaQQ.SDK.MainPanels;
using AvaQQ.ViewModels.MainPanels;
using AvaQQ.Views.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using MainPanelConfig = AvaQQ.SDK.Configuration<AvaQQ.Configurations.MainPanelConfiguration>;

namespace AvaQQ.MainPanels;

internal class FriendCategorySelection : ICategorySelection
{
	private readonly object _lock = new();

	private readonly ILogger<FriendCategorySelection> _logger;

	private FriendListView? _view;

	private readonly Timer _timer;

	public FriendCategorySelection(IServiceProvider serviceProvider)
	{
		_logger = serviceProvider.GetRequiredService<ILogger<FriendCategorySelection>>();
		_timer = new(DestroyView);
	}

	public UserControl? UserControl
	{
		get
		{
			lock (_lock)
			{
				if (_view is null)
				{
					_view = new FriendListView()
					{
						DataContext = new FriendListViewModel(),
					};
					_logger.LogInformation("FriendListView has been created.");
				}

				return _view;
			}
		}
	}

	private void DestroyView(object? state)
	{
		lock (_lock)
		{
			_view = null;
			_timer.Change(Timeout.Infinite, Timeout.Infinite);
			_logger.LogInformation("FriendListView has been destroyed.");
		}
	}

	public override string ToString()
	{
		return SR.TextFriend;
	}

	public void OnSelected()
	{
		_logger.LogInformation("FriendListView has been selected.");
		_timer.Change(Timeout.Infinite, Timeout.Infinite);
		_logger.LogInformation("FriendListView has been stopped from destruction.");
	}

	public void OnDeselected()
	{
		_logger.LogInformation("FriendListView has been deselected.");
		_timer.Change(MainPanelConfig.Instance.UnusedViewDestructionTime, Timeout.InfiniteTimeSpan);
		_logger.LogInformation(
			"FriendListView has been scheduled for destruction after {Delay}.",
			MainPanelConfig.Instance.UnusedViewDestructionTime
		);
	}
}
