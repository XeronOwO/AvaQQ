using Microsoft.Extensions.Hosting;
using System;

namespace AvaQQ.SDK;

/// <summary>
/// 插件基类。
/// </summary>
public class Plugin
{
	/// <summary>
	/// 预加载时调用。<br/>
	/// 在此处添加主机构建器的配置，例如注册一些服务，以供自己或其它插件使用 <see cref="IServiceProvider"/> 调用。<br/>
	/// 注意：请不要写过于复杂的逻辑，这部分是没有日志记录器的，日志无法保存到文件，只能通过 <see cref="System.Diagnostics.Debug"/> 在调试器查看。
	/// </summary>
	public virtual void OnPreload(IHostBuilder hostBuilder)
	{
	}

	/// <summary>
	/// 加载时调用。<br/>
	/// 可以通过 <paramref name="services"/> 获取注册的服务。可以是 AvaQQ 的服务，可以是自己注册的，也可以是其它插件注册的。<br/>
	/// 例如，你可以用类似 <c>services.GetRequiredService&lt;ILogger&lt;Plugin&gt;&gt;()</c> 的方法获取日志记录器，然后记录日志。
	/// </summary>
	public virtual void OnLoad(IServiceProvider services)
	{
	}

	/// <summary>
	/// 卸载时调用。
	/// </summary>
	public virtual void OnUnload()
	{
	}
}
