using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AvaQQ.SDK.Logging;

/// <summary>
/// Extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class LoggingBuilderExtensions
{
	/// <summary>
	/// Configures the logging system to use the specified <see cref="LogRecorder"/>.
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/> to add the logging services to.</param>
	/// <param name="recorder">The <see cref="LogRecorder"/> to use.</param>
	public static IServiceCollection ConfigureRecordLogger(this IServiceCollection services, LogRecorder recorder)
	{
		return services.AddLogging(builder =>
		{
			builder.ClearProviders();
			builder.AddProvider(new RecordLoggerProvider(recorder));
		});
	}
}
