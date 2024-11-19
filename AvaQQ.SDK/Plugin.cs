using Microsoft.Extensions.Hosting;
using System;

namespace AvaQQ.SDK;

/// <summary>
/// Represents a plugin.
/// </summary>
public class Plugin
{
	/// <summary>
	/// Called when the plugin is pre-loaded.
	/// </summary>
	/// <param name="hostBuilder">The host builder.<br/>Use this to register services, etc.</param>
	public virtual void OnPreload(IHostBuilder hostBuilder)
	{
	}

	/// <summary>
	/// Called when the plugin is loaded.
	/// </summary>
	/// <param name="serviceProvider">The service provider.<br/>Use this to get services, etc.</param>
	public virtual void OnLoad(IServiceProvider serviceProvider)
	{
	}

	/// <summary>
	/// Called when the plugin is unloaded.
	/// </summary>
	public virtual void OnUnload()
	{
	}
}
