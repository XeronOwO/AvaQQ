using Microsoft.Extensions.Logging;

namespace AvaQQ.SDK.Logging;

internal class RecordLogger(
	LogRecorder recorder,
	ILoggerProvider? linkedLoggerProvider,
	string name) : ILogger
{
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
	{
		return default;
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return true;
	}

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

		recorder.OnLog(name, DateTime.Now, logLevel, eventId, state, exception, formatter);
		linkedLoggerProvider?.CreateLogger(name).Log(logLevel, eventId, state, exception, formatter);
	}
}
