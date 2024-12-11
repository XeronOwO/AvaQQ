using Avalonia.Controls;
using AvaQQ.SDK.Adapters;

namespace AvaQQ.SDK.Views;

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
	/// 当结束连接时
	/// </summary>
	/// <param name="adapter">
	/// 适配器<br/>
	/// 如果连接失败，则设置 null，会恢复控件状态，不会关闭窗口<br/>
	/// 如果连接成功，则设置适配器实例，会关闭窗口并触发 <see cref="AppBase.OnConnected(IAdapter)"/> 事件
	/// </param>
	public abstract void EndConnect(IAdapter? adapter);
}
