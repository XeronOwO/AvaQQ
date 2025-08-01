using AvaQQ.Core.Adapters;
using AvaQQ.Core.Databases;
using AvaQQ.Core.Events;
using AvaQQ.Core.MainPanels;
using AvaQQ.Core.Views.Connecting;
using AvaQQ.Core.Views.MainPanels;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AvaQQ.Core;

internal static class CorePluginRegistration
{
	public static IHostBuilder ConfigureAvaQQCore(this IHostBuilder hostBuilder)
	{
		hostBuilder.ConfigureServices(services =>
		{
			services.AddSingleton<AppBase, App>()
				// 连接窗口
				.AddScoped<SelectAdapterSelection>()
				.AddTransient<ConnectView>()
				.AddScoped<ConnectWindow>()
				// 主面板窗口
				.AddScoped<RecentCategorySelection>()
				.AddScoped<FriendCategorySelection>()
				.AddScoped<GroupCategorySelection>()
				.AddSingleton<ICategorySelectionProvider, CategorySelectionProvider>()
				.AddTransient<FriendListView>()
				.AddTransient<GroupListView>()
				.AddTransient<HeaderView>()
				.AddTransient<CategorizedListView>()
				.AddTransient<MainPanelView>()
				.AddScoped<MainPanelWindow>()
				// 其它单例服务
				.AddSingleton<IAppLifetime, AppLifetimeController>()
				.AddSingleton<IAdapterProvider, AdapterProvider>()
				.AddSingleton<IAdapterSelectionProvider, AdapterSelectionProvider>()
				.AddSingleton<IDatabase, SqliteDatabase>()
				.AddSingleton<IAvatarCacheProvider, AvatarCacheProvider>()
				.AddSingleton<EventStation>()
				;
		});

		return hostBuilder;
	}
}
