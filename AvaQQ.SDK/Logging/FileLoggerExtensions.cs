using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace AvaQQ.SDK.Logging;

/// <summary>
/// 文件日志记录器扩展
/// </summary>
public static class FileLoggerExtensions
{
	/// <summary>
	/// 添加文件日志记录器
	/// </summary>
	public static ILoggingBuilder AddFileLogger(
		this ILoggingBuilder builder)
	{
		builder.AddConfiguration();

		builder.Services.TryAddEnumerable(
			ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>()
		);

		LoggerProviderOptions.RegisterProviderOptions<LoggerFilterOptions, FileLoggerProvider>(builder.Services);

		return builder;
	}
}
