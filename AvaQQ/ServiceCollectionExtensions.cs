using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAvaQQ(this IServiceCollection services)
		=> services.AddSingleton<App>()
			.AddSingleton<ILifetimeController, LifetimeController>();
}
