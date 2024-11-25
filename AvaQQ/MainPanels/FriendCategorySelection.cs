﻿using Avalonia.Controls;
using AvaQQ.Resources;
using AvaQQ.SDK.MainPanels;
using AvaQQ.Utils;
using AvaQQ.ViewModels.MainPanels;
using AvaQQ.Views.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using MainPanelConfig = AvaQQ.SDK.Configuration<AvaQQ.Configurations.MainPanelConfiguration>;

namespace AvaQQ.MainPanels;

internal class FriendCategorySelection : ICategorySelection
{
	private readonly object _lock = new();

	private readonly ILogger<FriendCategorySelection> _logger;

	private FriendListView? _view;

	private readonly Watchdog _watchdog;

	public FriendCategorySelection(IServiceProvider serviceProvider)
	{
		_logger = serviceProvider.GetRequiredService<ILogger<FriendCategorySelection>>();
		_watchdog = new(DestroyView);
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
			_watchdog.Stop();
			_logger.LogInformation("FriendListView has been destroyed.");
		}
	}

	public override string ToString()
	{
		return SR.TextFriend;
	}

	public void OnSelected()
	{
		_watchdog.Stop();
		_logger.LogInformation("FriendListView has been stopped from destruction.");
	}

	public void OnDeselected()
	{
		_watchdog.Start(MainPanelConfig.Instance.UnusedViewDestructionTime);
		_logger.LogInformation(
			"FriendListView has been scheduled for destruction after {Delay}.",
			MainPanelConfig.Instance.UnusedViewDestructionTime
		);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
			}

			_watchdog.Dispose();
			disposedValue = true;
		}
	}

	~FriendCategorySelection()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion
}
