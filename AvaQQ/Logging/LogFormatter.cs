using Microsoft.Extensions.Logging;
using System.Text;

namespace AvaQQ.Logging;

public static class LogFormatter
{
	public static string Format<TState>(
		string name,
		DateTime time,
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
		=> Format(name, time, logLevel, eventId, state, exception, formatter(state, exception));

	public static string Format<TState>(
		string name,
		DateTime time,
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		string message)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{GetLogLevelString(logLevel)}: {name}[{eventId.Id}] @ {time}");
		sb.AppendLine($"      {message}");
		if (exception is not null)
		{
			sb.AppendLine(exception.ToString());
		}

		return sb.ToString();
	}

	public static string GetLogLevelString(LogLevel logLevel)
	{
		return logLevel switch
		{
			LogLevel.Trace => "trce",
			LogLevel.Debug => "dbug",
			LogLevel.Information => "info",
			LogLevel.Warning => "warn",
			LogLevel.Error => "fail",
			LogLevel.Critical => "crit",
			_ => throw new ArgumentOutOfRangeException(nameof(logLevel))
		};
	}
}
