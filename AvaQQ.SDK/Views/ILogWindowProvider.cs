using Avalonia.Controls;
using System.Threading.Tasks;

namespace AvaQQ.SDK.Views;

/// <summary>
/// 日志窗口提供者
/// </summary>
public interface ILogWindowProvider
{
	/// <summary>
	/// 显示日志
	/// </summary>
	/// <param name="log">日志内容</param>
	void Show(string log);

	/// <summary>
	/// 显示日志
	/// </summary>
	/// <param name="window">父窗口</param>
	/// <param name="log">日志内容</param>
	Task ShowDialog(Window window, string log);
}
