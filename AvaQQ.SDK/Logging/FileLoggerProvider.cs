using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace AvaQQ.SDK.Logging;

internal class FileLoggerProvider : ILoggerProvider
{
	private static readonly string _providerTypeFullName = typeof(FileLoggerProvider).FullName!;

	private readonly IDisposable? _onChangeToken;

	private LoggerFilterOptions _currentConfig;

	private readonly ConcurrentDictionary<string, FileLogger> _loggers =
		new(StringComparer.OrdinalIgnoreCase);

	public FileLoggerProvider(IOptionsMonitor<LoggerFilterOptions> config)
	{
		_currentConfig = config.CurrentValue;
		_onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
	}

	public ILogger CreateLogger(string categoryName)
		=> _loggers.GetOrAdd(
			categoryName,
			name => new FileLogger(_providerTypeFullName, name, GetCurrentConfig)
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

	~FileLoggerProvider()
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
