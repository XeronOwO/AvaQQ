using Avalonia.Controls;
using AvaQQ.Core.Resources;
using AvaQQ.Core.Utils;
using AvaQQ.Core.Views.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.MainPanelConfiguration>;

namespace AvaQQ.Core.MainPanels;

internal class FriendCategorySelection : ICategorySelection
{
	private readonly object _lock = new();

	private readonly IServiceProvider _serviceProvider;

	private readonly ILogger<FriendCategorySelection> _logger;

	private FriendListView? _view;

	private readonly Watchdog _watchdog;

	public FriendCategorySelection(
		IServiceProvider serviceProvider,
		ILogger<FriendCategorySelection> logger
		)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
		_watchdog = new(DestroyView);
	}

	public UserControl? View
	{
		get
		{
			lock (_lock)
			{
				if (_view is null)
				{
					_view = _serviceProvider.GetRequiredService<FriendListView>();
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
		_watchdog.Start(Config.Instance.UnusedViewDestructionTime);
		_logger.LogInformation(
			"FriendListView has been scheduled for destruction after {Delay}.",
			Config.Instance.UnusedViewDestructionTime
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
				_watchdog.Dispose();
			}

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
