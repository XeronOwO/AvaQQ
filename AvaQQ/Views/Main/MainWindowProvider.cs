using Avalonia.Controls;
using AvaQQ.Events;
using AvaQQ.Resources;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AvaQQ.Views.Main;

internal class MainWindowProvider : IMainWindowProvider, IDisposable
{
	private readonly ILogger<MainWindowProvider> _logger;

	private readonly IAdapterProvider _adapterProvider;

	private readonly IServiceProvider _serviceProvider;

	private readonly AvaQQEvents _events;

	public MainWindowProvider(
		ILogger<MainWindowProvider> logger,
		IAdapterProvider adapterProvider,
		IServiceProvider serviceProvider,
		AvaQQEvents events)
	{
		_logger = logger;
		_adapterProvider = adapterProvider;
		_serviceProvider = serviceProvider;
		_events = events;

		_events.OnTrayIconClicked.Subscribe(OnTrayIconClicked);
	}

	private IServiceScope? _scope;

	public IServiceScope? Scope => _scope;

	private MainWindow? _window;

	[NotNullIfNotNull(nameof(Scope))]
	public MainWindow? Window => _window;

	Window? IMainWindowProvider.Window => Window;

	public void OpenOrActivateMainWindow()
	{
		if (_adapterProvider.Adapter is null)
		{
			throw new InvalidOperationException(SR.ExceptionOpenMainWindow_AdapterIsNull);
		}

		if (_window is not null)
		{
			_window.Activate();
			_logger.LogInformation($"{nameof(MainWindow)} is already open. Activating it");
			return;
		}

		_scope = _serviceProvider.CreateScope();
		_window = _scope.ServiceProvider.GetRequiredService<MainWindow>();
		_window.Show();

		_window.Closed += MainWindow_Closed;
		_logger.LogInformation($"{nameof(MainWindow)} opened");
	}

	private void MainWindow_Closed(object? sender, EventArgs e)
	{
		Debug.Assert(_window is not null, nameof(_window) + " is null");
		if (_window is null)
		{
			_logger.LogError($"{nameof(MainWindow)} is null when closed. This should not happen");
			return;
		}

		_window.Closed -= MainWindow_Closed;
		_logger.LogInformation($"{nameof(MainWindow)} closed");
		_window = null;
		_scope?.Dispose();
		_scope = null;
	}

	private void OnTrayIconClicked(object? sender, EventBusArgs<EmptyEventResult> e)
	{
		OpenOrActivateMainWindow();
		e.IsHandled = true;
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_events.OnTrayIconClicked.Unsubscribe(OnTrayIconClicked);

			if (disposing)
			{
				_scope?.Dispose();
			}

			disposedValue = true;
		}
	}

	~MainWindowProvider()
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
