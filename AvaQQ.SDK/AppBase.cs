using Avalonia;
using Avalonia.Controls;
using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.Resources;
using AvaQQ.SDK.ViewModels;
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
	/// 初始化 <see cref="AppBase"/> 类的新实例。
	/// </summary>
	public AppBase(ILogger<AppBase> logger)
	{
		_logger = logger;

		AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
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
	public abstract IAdapter? Adapter { get; set; }

	/// <summary>
	/// 连接窗口
	/// </summary>
	public abstract ConnectWindowBase? ConnectWindow { get; set; }

	/// <summary>
	/// 主面板窗口
	/// </summary>
	public abstract Window? MainPanelWindow { get; set; }

	private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		try
		{
			_logger.LogCritical((Exception)e.ExceptionObject, "Unhandled exception.");
		}
		catch
		{
		}

#if DEBUG
		throw (Exception)e.ExceptionObject;
#else
		if (e.IsTerminating)
		{
			Lifetime.Stop();
		}
#endif
	}
}
