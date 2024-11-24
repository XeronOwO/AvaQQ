using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.SDK.MainPanels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ.Views.MainPanels;

public partial class CategorizedListView : UserControl
{
	public CategorizedListView()
	{
		InitializeComponent();

		Loaded += CategorizedListView_Loaded;
		categorySelectionView.SelectionChanged += CategorySelectionView_SelectionChanged;
	}

	private void CategorizedListView_Loaded(object? sender, RoutedEventArgs e)
	{
		if (Application.Current is not App app)
		{
			return;
		}

		categorySelectionView.Items.Clear();
		var selections = app.ServiceProvider.GetRequiredService<ICategorySelectionProvider>();
		foreach (var selection in selections.CreateSelections())
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
}
