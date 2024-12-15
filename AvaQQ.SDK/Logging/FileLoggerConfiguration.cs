using Microsoft.Extensions.Logging;

namespace AvaQQ.SDK.Logging;

internal class FileLoggerConfiguration
{
	public LogLevel LogLevel { get; set; } = LogLevel.Information;
}
