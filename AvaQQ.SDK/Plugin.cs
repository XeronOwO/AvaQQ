using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;

namespace AvaQQ.SDK;

/// <summary>
/// 插件基类。
/// </summary>
public class Plugin
{
	/// <summary>
	/// 预加载时调用。<br/>
	/// 在此处添加主机构建器的配置，例如注册一些服务，以供自己或其它插件使用 <see cref="IServiceProvider"/> 调用。<br/>
	/// 注意：请不要写过于复杂的逻辑，这部分是没有日志记录器的，日志无法保存到文件，只能通过 <see cref="Debug"/> 在调试器查看。
	/// </summary>
	public virtual void OnPreload(IHostBuilder hostBuilder)
	{
	}

	/// <summary>
	/// 加载时调用。<br/>
	/// 推荐在这里初始化插件自身的资源，并为后续 <see cref="OnPostLoad"/> 的资源交换做准备。<br/>
	/// 可以通过 <paramref name="services"/> 获取注册的服务。<br/>
	/// 例如，你可以用类似 <c>services.GetRequiredService&lt;ILogger&lt;Plugin&gt;&gt;()</c> 的方法获取日志记录器，然后记录日志。
	/// </summary>
	public virtual void OnLoad(IServiceProvider services)
	{
	}

	/// <summary>
	/// 加载后调用。<br/>
	/// 推荐在这里进行资源交换，例如获取其它插件的资源并使用。<br/>
	/// 可以通过 <paramref name="services"/> 获取注册的服务。
	/// </summary>
	public virtual void OnPostLoad(IServiceProvider services)
	{
	}

	/// <summary>
	/// 卸载时调用。
	/// </summary>
	public virtual void OnUnload()
	{
	}
}
