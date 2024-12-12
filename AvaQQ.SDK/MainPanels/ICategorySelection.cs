using Avalonia.Controls;
using System;

namespace AvaQQ.SDK.MainPanels;

/// <summary>
/// 分类选项<br/>
/// 注：选项文本由 <see cref="object.ToString"/> 方法获取，请记得重写该方法。
/// </summary>
public interface ICategorySelection : IDisposable
{
	/// <summary>
	/// 获取选中该分类后展示的界面，也就是说需要自己DIY，可以参考已实现的分类界面的有关代码。
	/// </summary>
	UserControl? View { get; }

	/// <summary>
	/// 选中该分类时触发
	/// </summary>
	void OnSelected();

	/// <summary>
	/// 取消选中该分类时触发
	/// </summary>
	void OnDeselected();
}
