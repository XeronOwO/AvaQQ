using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.SDK;
using AvaQQ.SDK.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AvaQQ.Views.MainPanels;

public partial class CategorizedListView : UserControl
{
	private readonly IServiceScope _serviceScope;

	public CategorizedListView(IServiceProvider serviceProvider)
	{
		InitializeComponent();

		_serviceScope = serviceProvider.CreateScope();

		Loaded += CategorizedListView_Loaded;
		categorySelectionView.SelectionChanged += CategorySelectionView_SelectionChanged;
		Unloaded += CategorizedListView_Unloaded;
	}

	public CategorizedListView() : this(DesignerServiceProviderHelper.Root)
	{
	}

	private void CategorizedListView_Loaded(object? sender, RoutedEventArgs e)
	{
		categorySelectionView.Items.Clear();
		var selections = _serviceScope.ServiceProvider.GetRequiredService<ICategorySelectionProvider>();
		foreach (var selection in selections.CreateSelections(_serviceScope.ServiceProvider))
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
		if (selection.UserControl is { } control)
		{
			gridContent.Children.Add(control);
		}
	}

	private void CategorizedListView_Unloaded(object? sender, RoutedEventArgs e)
	{
		_serviceScope.Dispose();
	}
}
