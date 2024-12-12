using Avalonia.Controls;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ.Views.MainPanels;

public partial class MainPanelView : UserControl
{
	public MainPanelView(
		HeaderView headerView,
		CategorizedListView categorizedListView
		)
	{
		InitializeComponent();

		gridHeaderView.Children.Add(headerView);
		gridCategorizedListView.Children.Add(categorizedListView);
	}

	public MainPanelView() : this(
		DesignerServiceProviderHelper.Root.GetRequiredService<HeaderView>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<CategorizedListView>()
		)
	{
	}
}
