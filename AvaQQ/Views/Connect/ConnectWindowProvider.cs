using AvaQQ.Events;
using AvaQQ.Resources;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AvaQQ.Views.Connect;

internal class ConnectWindowProvider : IConnectWindowProvider, IDisposable
{
	private readonly ILogger<ConnectWindowProvider> _logger;

	private readonly IAdapterProvider _adapterProvider;

	private readonly IServiceProvider _serviceProvider;

	private readonly AvaQQEvents _events;

	public ConnectWindowProvider(
		ILogger<ConnectWindowProvider> logger,
		IAdapterProvider adapterProvider,
		IServiceProvider serviceProvider,
		AvaQQEvents events
	)
	{
		_logger = logger;
		_adapterProvider = adapterProvider;
		_serviceProvider = serviceProvider;
		_events = events;

		_events.OnTrayIconClicked.Subscribe(OnTrayIconClicked);
	}

	private IServiceScope? _scope;

	public IServiceScope? Scope => _scope;

	private ConnectWindow? _window;

	[NotNullIfNotNull(nameof(Scope))]
	public ConnectWindow? Window => _window;

	ConnectWindowBase? IConnectWindowProvider.Window => Window;

	public void OpenConnectWindow()
	{
		if (_adapterProvider.Adapter is not null)
		{
			throw new InvalidOperationException(SR.ExceptionOpenConnectWindow_AdapterAlreadySet);
		}

		_scope = _serviceProvider.CreateScope();
		_window = _scope.ServiceProvider.GetRequiredService<ConnectWindow>();
		_window.Show();

		_window.Closed += ConnectWindow_Closed;
		_logger.LogInformation($"{nameof(ConnectWindow)} opened");
	}

	private void ConnectWindow_Closed(object? sender, EventArgs e)
	{
		Debug.Assert(_window is not null, nameof(_window) + " is null");
		if (_window is null)
		{
			_logger.LogError($"{nameof(ConnectWindow)} is null when closed. This should not happen");
			return;
		}

		_window.Closed -= ConnectWindow_Closed;
		_logger.LogInformation($"{nameof(ConnectWindow)} closed");
		_window = null;
		_scope?.Dispose();
		_scope = null;
	}

	private void OnTrayIconClicked(object? sender, EventBusArgs<EmptyEventResult> e)
	{
		if (_adapterProvider.Adapter is not null)
		{
			_logger.LogTrace("Tray icon clicked, but adapter is already set. Ignoring click");
			return;
		}

		_logger.LogInformation("Tray icon clicked. Activating or opening connect window...");
		if (_window is not null)
		{
			_window.Activate();
			e.IsHandled = true;
			return;
		}

		Debug.Assert(false, "This should not happen, as the connect window should be opened when the app starts");
		OpenConnectWindow();
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

	~ConnectWindowProvider()
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
