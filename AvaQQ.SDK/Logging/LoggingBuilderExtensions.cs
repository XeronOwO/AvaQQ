using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AvaQQ.SDK.Logging;

/// <summary>
/// 日志构建器扩展
/// </summary>
public static class LoggingBuilderExtensions
{
	/// <summary>
	/// 配置记录日志器
	/// </summary>
	/// <param name="services">服务</param>
	/// <param name="recorder">日志记录器</param>
	public static IServiceCollection ConfigureRecordLogger(this IServiceCollection services, LogRecorder recorder)
	{
		return services.AddLogging(builder =>
		{
			builder.ClearProviders();
			builder.AddProvider(new RecordLoggerProvider(recorder));
		});
	}
}
