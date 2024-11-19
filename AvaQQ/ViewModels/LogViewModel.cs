using AvaQQ.SDK.Logging;
using AvaQQ.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace AvaQQ.ViewModels;

public class LogViewModel : ViewModelBase
{
	public string TextLogs => SR.TextLogs;

	public string TextCopy => SR.TextCopy;

	public LogRecorder Logs { get; init; } = new();

	public string LogsText
	{
		get
		{
			var sb = new StringBuilder();

			foreach (var record in Logs.Records)
			{
				sb.Append(GetLogLevelString(record.LogLevel))
					.Append(": ")
					.Append(record.Name)
					.Append('[')
					.Append(record.EventId.Id)
					.AppendLine("]")
					.Append("      ")
					.AppendLine(record.Message)
					;
			}

			return sb.ToString();
		}
	}

	private static string GetLogLevelString(LogLevel logLevel)
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
