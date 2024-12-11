using Avalonia;
using Avalonia.Controls;
using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.Resources;
using Microsoft.Extensions.Logging;
using System;

namespace AvaQQ.SDK;

/// <summary>
/// 应用程序的基类
/// </summary>
public abstract class AppBase : Application
{
	private readonly ILogger<AppBase> _logger;

	/// <summary>
	/// 生命周期控制器
	/// </summary>
	protected readonly IAppLifetimeController _lifetime;

	/// <summary>
	/// 初始化 <see cref="AppBase"/> 类的新实例。
	/// </summary>
	public AppBase(ILogger<AppBase> logger, IAppLifetimeController lifetime)
	{
		_logger = logger;
		_lifetime = lifetime;

#if !DEBUG
		AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
		{
			try
			{
				_logger.LogCritical((Exception)e.ExceptionObject, "Unhandled exception.");
			}
			catch
			{
			}

			if (e.IsTerminating)
			{
				_lifetime.Stop();
			}
		};
#endif
	}

	/// <summary>
	/// 获取当前应用程序的实例<br/>
	/// 如果当前应用程序不是 <see cref="AppBase"/> 类型，则会抛出异常。
	/// </summary>
	public static new AppBase Current
	{
		get
		{
			if (Application.Current is not { } app)
			{
				throw new InvalidOperationException(SR.ExceptionCurrentApplicationIsNull);
			}
			if (app is not AppBase appBase)
			{
				throw new InvalidOperationException(SR.ExceptionUnsupportedApplicationType);
			}
			return appBase;
		}
	}

	/// <summary>
	/// 获取应用程序的适配器。
	/// </summary>
	public IAdapter? Adapter { get; protected set; }

	/// <summary>
	/// 获取或设置主窗口，点击托盘图标时会聚焦该窗口
	/// </summary>
	public Window? MainWindow { get; set; }

	/// <summary>
	/// 当连接时触发<br/>
	/// 此方法由连接窗口调用，请勿手动调用
	/// </summary>
	/// <param name="adapter">
	/// 适配器<br/>
	/// 如果连接失败，设置为 null，应用将会退出<br/>
	/// 如果连接成功，设置为适配器实例，将会打开主面板窗口
	/// </param>
	public abstract void OnConnected(IAdapter? adapter);

	/// <summary>
	/// 主面板窗口
	/// </summary>
	public abstract Window? MainPanelWindow { get; set; }
}
