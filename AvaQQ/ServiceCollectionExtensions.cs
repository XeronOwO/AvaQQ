using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAvaQQ(this IServiceCollection services)
		=> services.AddSingleton<AppBase, App>()
			.AddSingleton<ILifetimeController, LifetimeController>();
}
