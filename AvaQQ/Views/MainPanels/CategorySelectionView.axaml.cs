using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AvaQQ.Views.MainPanels;

public partial class CategorySelectionView : UserControl
{
	public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
		RoutedEvent.Register<CategorySelectionView, SelectionChangedEventArgs>(
			nameof(SelectionChanged),
			RoutingStrategies.Bubble);

	public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
	{
		add => AddHandler(SelectionChangedEvent, value);
		remove => RemoveHandler(SelectionChangedEvent, value);
	}

	public ObservableCollection<object> Items { get; } = [];

	private object? _selectedItem;

	public object? SelectedItem
	{
		get => _selectedItem;
		set
		{
			if (_selectedItem == value)
			{
				return;
			}

			var oldValue = _selectedItem;
			_selectedItem = value;

			var newIndex = SelectedIndex;
			if (newIndex >= 0
				&& stackPanelCategory.Children[newIndex] is CategoryButton newButton)
			{
				newButton.IsSelected = true;
			}

			if (_selectedItem != oldValue)
			{
				IList addedItems = value is null ? Array.Empty<object>() : [value];
				IList removedItems = oldValue is null ? Array.Empty<object>() : [oldValue];
				RaiseEvent(new SelectionChangedEventArgs(
					SelectionChangedEvent,
					removedItems,
					addedItems
				));
			}
		}
	}

	public int SelectedIndex
	{
		get => SelectedItem is null ? -1 : Items.IndexOf(SelectedItem);
		set
		{
			if (value < 0 || value >= Items.Count)
			{
				return;
			}
			SelectedItem = Items[value];
		}
	}

	public CategorySelectionView()
	{
		Items.CollectionChanged += Items_CollectionChanged;

		InitializeComponent();
	}

	private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		switch (e.Action)
		{
			case NotifyCollectionChangedAction.Add:
				AddItem(e);
				break;
			case NotifyCollectionChangedAction.Remove:
				RemoveItem(e);
				break;
			case NotifyCollectionChangedAction.Replace:
				ReplaceItem(e);
				break;
			case NotifyCollectionChangedAction.Move:
				MoveItem(e);
				break;
			case NotifyCollectionChangedAction.Reset:
				ResetItems(e);
				break;
			default:
				break;
		}
	}

	private void AddItem(NotifyCollectionChangedEventArgs e)
	{
		if (e.NewItems is null)
		{
			return;
		}

		var index = e.NewStartingIndex;
		foreach (var item in e.NewItems)
		{
			var button = new CategoryButton()
			{
				Content = item,
			};
			button.Selected += CategoryButton_Selected; ;
			stackPanelCategory.Children.Insert(index++, button);
		}
	}

	private void CategoryButton_Selected(object? sender, RoutedEventArgs e)
	{
		if (sender is CategoryButton button)
		{
			SelectedItem = button.Content;
		}
	}

	private void RemoveItem(NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems is null)
		{
			return;
		}

		for (int i = 0; i < e.OldItems.Count; i++)
		{
			stackPanelCategory.Children.RemoveAt(e.OldStartingIndex);
		}
	}

	private void ReplaceItem(NotifyCollectionChangedEventArgs e)
	{
		RemoveItem(e);
		AddItem(e);
	}

	private void MoveItem(NotifyCollectionChangedEventArgs e)
	{
		throw new NotImplementedException();
	}

	private void ResetItems(NotifyCollectionChangedEventArgs e)
	{
		stackPanelCategory.Children.Clear();
	}

	private void ScrollViewerCategory_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
	{
		if (e.Delta.X != 0 || e.Delta.Y == 0)
		{
			return;
		}

		// 滚轮可水平滚动窗口，不需要 shift+滚轮
		var offset = scrollViewerCategory.Offset;
		scrollViewerCategory.Offset = new(offset.X - e.Delta.Y * 32, offset.Y);
		e.Handled = true;
	}
}
