using Microsoft.Extensions.Logging;

namespace AvaQQ.Logging;

internal class Logger(
	string providerTypeFullName,
	string category,
	Func<LoggerFilterOptions> getCurrentConfig,
	LogToFileExecutor executor
	) : ILogger
{
	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		=> default;

	public bool IsEnabled(LogLevel level)
	{
		var config = getCurrentConfig();
		foreach (var rule in config.Rules)
		{
			var isCategoryPassed = string.IsNullOrEmpty(rule.CategoryName) ||
				rule.CategoryName == category;
			if (!isCategoryPassed)
			{
				continue;
			}

			var isFilterPassed = rule.Filter is null ||
				rule.Filter(providerTypeFullName, category, level);
			if (!isFilterPassed)
			{
				continue;
			}

			var isLogLevelPassed = level >= rule.LogLevel;
			if (!isLogLevelPassed)
			{
				continue;
			}

			return true;
		}

		return level >= config.MinLevel;
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

		executor.Append(LogFormatter.Format(
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
