using AvaQQ.Adapters;
using AvaQQ.Logging;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAvaQQ(this IServiceCollection services)
		=> services.AddSingleton<AppBase, App>()
		.AddSingleton<ILifetimeController, LifetimeController>()
		.AddSingleton<IAdapterSelectionProvider, AdapterSelectionProvider>()
		.AddSingleton<ILogWindowProvider, LogWindowProvider>();
}
