using Avalonia;
using Avalonia.Controls;
using AvaQQ.SDK.Adapters;
using System;

namespace AvaQQ.SDK;

/// <summary>
/// 应用程序的基类。
/// </summary>
public abstract class AppBase : Application
{
	/// <summary>
	/// 服务提供器
	/// </summary>
	public abstract IServiceProvider ServiceProvider { get; set; }

	/// <summary>
	/// 生命周期控制器
	/// </summary>
	public abstract ILifetimeController Lifetime { get; set; }

	/// <summary>
	/// 获取应用程序的适配器。
	/// </summary>
	public abstract IAdapter? Adapter { get; set; }

	/// <summary>
	/// 连接窗口
	/// </summary>
	public abstract Window? ConnectWindow { get; set; }

	/// <summary>
	/// 主面板窗口
	/// </summary>
	public abstract Window? MainPanelWindow { get; set; }
}
