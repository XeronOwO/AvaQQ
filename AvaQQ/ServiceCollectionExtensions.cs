using AvaQQ.Adapters;
using AvaQQ.Caches;
using AvaQQ.Databases;
using AvaQQ.Logging;
using AvaQQ.MainPanels;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.Databases;
using AvaQQ.SDK.MainPanels;
using AvaQQ.SDK.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAvaQQ(this IServiceCollection services)
		=> services.AddSingleton<AppBase, App>()
		.AddSingleton<ILifetimeController, LifetimeController>()
		.AddSingleton<IAdapterSelectionProvider, AdapterSelectionProvider>()
		.AddSingleton<ILogWindowProvider, LogWindowProvider>()
		.AddSingleton<ICategorySelectionProvider, CategorySelectionProvider>()
		.AddScoped<RecentCategorySelection>()
		.AddScoped<FriendCategorySelection>()
		.AddScoped<GroupCategorySelection>()
		.AddSingleton<IAvatarCache, AvatarCache>()
		.AddSingleton<IFriendCache, FriendCache>()
		.AddSingleton<IUserCache, UserCache>()
		.AddSingleton<IGroupCache, GroupCache>()
		.AddScoped<SelectAdapterSelection>()
		.AddSingleton<GroupMessageDatabase, GroupMessageLiteDB>();
}
