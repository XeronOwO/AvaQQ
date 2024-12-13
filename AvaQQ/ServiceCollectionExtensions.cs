using AvaQQ.Adapters;
using AvaQQ.Caches;
using AvaQQ.Databases;
using AvaQQ.Logging;
using AvaQQ.MainPanels;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.Databases;
using AvaQQ.SDK.MainPanels;
using AvaQQ.SDK.Views;
using AvaQQ.Views.Connecting;
using AvaQQ.Views.MainPanels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddAvaQQ(this IServiceCollection services)
		=> services.AddSingleton<AppBase, App>()
		.AddTransient<ConnectView>()
		.AddScoped<ConnectWindowBase, ConnectWindow>()
		.AddTransient<HeaderView>()
		.AddTransient<CategorizedListView>()
		.AddTransient<MainPanelView>()
		.AddTransient<FriendListView>()
		.AddTransient<GroupListView>()
		.AddScoped<MainPanelWindow>()
		.AddSingleton<IAppLifetimeController, AppLifetimeController>()
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
