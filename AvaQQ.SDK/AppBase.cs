using Avalonia;
using Avalonia.Controls;
using AvaQQ.SDK.Resources;
using Microsoft.Extensions.Logging;

namespace AvaQQ.SDK;

/// <summary>
/// 应用程序的基类
/// </summary>
public abstract class AppBase : Application
{
	/// <summary>
	/// 生命周期控制器
	/// </summary>
	protected readonly IAppLifetimeController _lifetime;

	/// <summary>
	/// 初始化 <see cref="AppBase"/> 类的新实例。
	/// </summary>
	public AppBase(ILogger<AppBase> logger, IAppLifetimeController lifetime)
	{
		_lifetime = lifetime;

#if !DEBUG
		AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
		{
			try
			{
				logger.LogCritical((Exception)e.ExceptionObject, "Unhandled exception.");
			}
			catch
			{
			}

			if (e.IsTerminating)
			{
				lifetime.Stop();
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
	/// 主窗口<br/>
	/// 点击托盘图标时，会显示此窗口
	/// </summary>
	public Window? MainWindow { get; set; }
}
