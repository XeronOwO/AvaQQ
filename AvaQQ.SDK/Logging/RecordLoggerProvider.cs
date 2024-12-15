using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace AvaQQ.SDK.Logging;

internal class RecordLoggerProvider(
	LogRecorder recorder,
	ILoggerProvider? linkedLoggerProvider = null) : ILoggerProvider
{
	private readonly ConcurrentDictionary<string, RecordLogger> _loggers =
		new(StringComparer.OrdinalIgnoreCase);

	public ILogger CreateLogger(string categoryName)
	{
		return _loggers.GetOrAdd(
			categoryName,
			name => new RecordLogger(
				recorder,
				linkedLoggerProvider,
				name
			)
		);
	}

	public void Dispose()
	{
	}
}
