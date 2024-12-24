using Microsoft.Extensions.Logging;

namespace AvaQQ.SDK.Logging;

internal class FileLogger(
	string providerTypeFullName,
	string category,
	Func<LoggerFilterOptions> getCurrentConfig
	) : ILogger
{
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		=> default;

	public bool IsEnabled(LogLevel level)
	{
		var config = getCurrentConfig();
		foreach (var rule in config.Rules)
		{
			if (level < config.MinLevel)
			{
				continue;
			}

			if (rule.Filter != null)
			{
				if (rule.Filter(providerTypeFullName, category, level))
				{
					return true;
				}
				continue;
			}
		}

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

		FileLoggingExecutor.Append(SimpleLogFormatter.Format(
			category,
			DateTime.Now,
			logLevel,
			eventId,
			state,
			exception,
			formatter
		));
	}
}
