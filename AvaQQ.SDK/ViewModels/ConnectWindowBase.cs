using Avalonia.Controls;
using AvaQQ.SDK.Adapters;

namespace AvaQQ.SDK.ViewModels;

/// <summary>
/// 连接窗口
/// </summary>
public abstract class ConnectWindowBase : Window
{
	/// <summary>
	/// 当开始连接时<br/>
	/// 会禁用界面上的按钮等可操作控件
	/// </summary>
	public abstract void BeginConnect();

	/// <summary>
	/// 当结束连接时<br/>
	/// 会启用界面上的按钮等可操作控件
	/// </summary>
	/// <param name="adapter">适配器</param>
	public abstract void EndConnect(IAdapter? adapter);
}
