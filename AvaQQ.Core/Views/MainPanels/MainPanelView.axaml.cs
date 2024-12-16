using Avalonia.Controls;
using AvaQQ.Core.Utils;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ.Core.Views.MainPanels;

/// <summary>
/// 主面板视图
/// </summary>
public partial class MainPanelView : UserControl
{
	/// <summary>
	/// 创建主面板视图
	/// </summary>
	public MainPanelView(
		HeaderView headerView,
		CategorizedListView categorizedListView
		)
	{
		CirculationInjectionDetector<MainPanelView>.Enter();

		InitializeComponent();

		gridHeaderView.Children.Add(headerView);
		gridCategorizedListView.Children.Add(categorizedListView);

		CirculationInjectionDetector<MainPanelView>.Leave();
	}

	/// <summary>
	/// 创建主面板视图
	/// </summary>
	public MainPanelView() : this(
		DesignerServiceProviderHelper.Root.GetRequiredService<HeaderView>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<CategorizedListView>()
		)
	{
	}
}
