using Microsoft.Extensions.Logging;

namespace AvaQQ.SDK.Logging;

internal class FileLogger(
	string name,
	Func<FileLoggerConfiguration> getCurrentConfig
	) : ILogger
{
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		=> default;

	public bool IsEnabled(LogLevel logLevel)
		=> logLevel >= getCurrentConfig().LogLevel;

	public void Log<TState>(
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}

		FileLoggingExecutor.Append(SimpleLogFormatter.Format(
			name,
			DateTime.Now,
			logLevel,
			eventId,
			state,
			exception,
			formatter
		));
	}
}
