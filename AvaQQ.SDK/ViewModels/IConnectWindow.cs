using AvaQQ.SDK.Adapters;

namespace AvaQQ.SDK.ViewModels;

/// <summary>
/// 连接窗口
/// </summary>
public interface IConnectWindow
{
	/// <summary>
	/// 当开始连接时
	/// </summary>
	void BeginConnect();

	/// <summary>
	/// 当结束连接时
	/// </summary>
	/// <param name="adapter">适配器</param>
	void EndConnect(IAdapter? adapter);
}
