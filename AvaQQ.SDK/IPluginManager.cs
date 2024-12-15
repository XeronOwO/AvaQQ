using Microsoft.Extensions.Hosting;

namespace AvaQQ.SDK;

/// <summary>
/// 插件管理器接口
/// </summary>
public interface IPluginManager
{
	/// <summary>
	/// 预加载插件
	/// </summary>
	void PreloadPlugins(IHostBuilder hostBuilder);

	/// <summary>
	/// 加载插件
	/// </summary>
	void LoadPlugins(IServiceProvider serviceProvider);

	/// <summary>
	/// 后加载插件
	/// </summary>
	void PostLoadPlugins(IServiceProvider serviceProvider);

	/// <summary>
	/// 卸载插件
	/// </summary>
	void UnloadPlugins(IServiceProvider serviceProvider);
}
