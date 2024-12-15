using Avalonia.Controls;

namespace AvaQQ.Core.Adapters;

/// <summary>
/// 协议适配器选项，用于在 <see cref="ComboBox"/> 中显示选项。<br/>
/// 注：选项文本由 <see cref="object.ToString"/> 方法获取，请记得重写该方法。
/// </summary>
public interface IAdapterSelection : IDisposable
{
	/// <summary>
	/// 获取选中该适配器后展示的配置界面，也就是说需要自己DIY，可以参考已实现的适配器的有关代码。
	/// </summary>
	UserControl? View { get; }

	/// <summary>
	/// 选中该适配器时触发
	/// </summary>
	void OnSelected();

	/// <summary>
	/// 取消选中该适配器时触发
	/// </summary>
	void OnDeselected();
}
