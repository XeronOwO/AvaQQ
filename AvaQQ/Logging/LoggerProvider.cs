using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace AvaQQ.Logging;

internal class LoggerProvider : ILoggerProvider
{
	private static readonly string _providerTypeFullName = typeof(LoggerProvider).FullName!;

	private readonly IDisposable? _onChangeToken;

	private LoggerFilterOptions _currentConfig;

	private readonly ConcurrentDictionary<string, Logger> _loggers =
		new(StringComparer.OrdinalIgnoreCase);

	private readonly LogToFileExecutor _executor;

	public LoggerProvider(
		IOptionsMonitor<LoggerFilterOptions> config,
		LogToFileExecutor executor
		)
	{
		_currentConfig = config.CurrentValue;
		_onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
		_executor = executor;
	}

	public ILogger CreateLogger(string categoryName)
		=> _loggers.GetOrAdd(
			categoryName,
			name => new Logger(_providerTypeFullName, name, GetCurrentConfig, _executor)
			);

	private LoggerFilterOptions GetCurrentConfig() => _currentConfig;

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_onChangeToken?.Dispose();
			}

			disposedValue = true;
		}
	}

	~LoggerProvider()
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
