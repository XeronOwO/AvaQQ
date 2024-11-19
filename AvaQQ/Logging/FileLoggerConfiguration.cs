using Microsoft.Extensions.Logging;

namespace AvaQQ.Logging;

public class FileLoggerConfiguration
{
	public LogLevel LogLevel { get; set; } = LogLevel.Information;
}
