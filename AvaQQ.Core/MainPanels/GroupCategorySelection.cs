using Avalonia.Controls;
using AvaQQ.Core.Resources;
using AvaQQ.Core.Utils;
using AvaQQ.Core.Views.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.MainPanelConfiguration>;

namespace AvaQQ.Core.MainPanels;

internal class GroupCategorySelection : ICategorySelection
{
	private readonly object _lock = new();

	private readonly IServiceProvider _serviceProvider;

	private readonly ILogger<GroupCategorySelection> _logger;

	private GroupListView? _view;

	private readonly Watchdog _watchdog;

	public GroupCategorySelection(
		IServiceProvider serviceProvider,
		ILogger<GroupCategorySelection> logger
		)
	{
		CirculationInjectionDetector<GroupCategorySelection>.Enter();

		_serviceProvider = serviceProvider;
		_logger = logger;
		_watchdog = new(DestroyView);

		CirculationInjectionDetector<GroupCategorySelection>.Leave();
	}

	public UserControl? View
	{
		get
		{
			lock (_lock)
			{
				if (_view is null)
				{
					_view = _serviceProvider.GetRequiredService<GroupListView>();
					_logger.LogInformation("GroupListView has been created.");
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
			_logger.LogDebug("GroupListView has been destroyed.");
		}
	}

	public override string ToString()
	{
		return SR.TextGroup;
	}

	public void OnSelected()
	{
		_watchdog.Stop();
		_logger.LogDebug("GroupListView has been stopped from destruction.");
	}

	public void OnDeselected()
	{
		_watchdog.Start(Config.Instance.UnusedViewDestructionTime);
		_logger.LogDebug(
			"GroupListView has been scheduled for destruction after {Delay}.",
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

	~GroupCategorySelection()
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
