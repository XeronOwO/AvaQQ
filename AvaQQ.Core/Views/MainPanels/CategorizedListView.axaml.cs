using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.Core.MainPanels;
using AvaQQ.Core.Utils;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ.Core.Views.MainPanels;

/// <summary>
/// 分类列表视图
/// </summary>
public partial class CategorizedListView : UserControl
{
	private readonly IServiceScope _serviceScope;

	/// <summary>
	/// 创建分类列表视图
	/// </summary>
	public CategorizedListView(IServiceProvider serviceProvider)
	{
		CirculationInjectionDetector<CategorizedListView>.Enter();

		InitializeComponent();

		_serviceScope = serviceProvider.CreateScope();

		Loaded += CategorizedListView_Loaded;
		categorySelectionView.SelectionChanged += CategorySelectionView_SelectionChanged;
		Unloaded += CategorizedListView_Unloaded;

		CirculationInjectionDetector<CategorizedListView>.Leave();
	}

	/// <summary>
	/// 创建分类列表视图
	/// </summary>
	public CategorizedListView() : this(DesignerServiceProviderHelper.Root)
	{
	}

	private void CategorizedListView_Loaded(object? sender, RoutedEventArgs e)
	{
		categorySelectionView.Items.Clear();

		var selectionProvider = _serviceScope.ServiceProvider.GetRequiredService<ICategorySelectionProvider>();
		foreach (var selection in selectionProvider.CreateSelections(_serviceScope.ServiceProvider))
		{
			categorySelectionView.Items.Add(selection);
		}
		if (categorySelectionView.Items.Count > 0)
		{
			categorySelectionView.SelectedIndex = 0;
		}
	}

	private void CategorySelectionView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (e.AddedItems.Count == 0
			|| e.AddedItems[0] is not ICategorySelection selection)
		{
			return;
		}

		gridContent.Children.Clear();
		if (selection.View is { } view)
		{
			gridContent.Children.Add(view);
		}
	}

	private void CategorizedListView_Unloaded(object? sender, RoutedEventArgs e)
	{
		_serviceScope.Dispose();
	}
}
