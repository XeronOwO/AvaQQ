using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaQQ.Core.MainPanels;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AvaQQ.Core.Views.MainPanels;

/// <summary>
/// 分类选择视图
/// </summary>
public partial class CategorySelectionView : UserControl
{
	/// <summary>
	/// 选项改变事件
	/// </summary>
	public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
		RoutedEvent.Register<CategorySelectionView, SelectionChangedEventArgs>(
			nameof(SelectionChanged),
			RoutingStrategies.Bubble);

	/// <summary>
	/// 当选项改变时触发
	/// </summary>
	public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
	{
		add => AddHandler(SelectionChangedEvent, value);
		remove => RemoveHandler(SelectionChangedEvent, value);
	}

	/// <summary>
	/// 选项集合
	/// </summary>
	public ObservableCollection<ICategorySelection> Items { get; } = [];

	private ICategorySelection? _selectedItem;

	/// <summary>
	/// 当前选中的选项
	/// </summary>
	public ICategorySelection? SelectedItem
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

	/// <summary>
	/// 当前选中的选项索引
	/// </summary>
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

	/// <summary>
	/// 创建分类选择视图
	/// </summary>
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

	private CategoryButton CreateCategoryButton(ICategorySelection selection)
	{
		var button = new CategoryButton()
		{
			Content = selection,
		};
		button.Selected += CategoryButton_Selected;
		return button;
	}

	private void AddItem(NotifyCollectionChangedEventArgs e)
	{
		if (e.NewItems is null
			|| e.NewStartingIndex < 0)
		{
			return;
		}

		var index = e.NewStartingIndex;
		foreach (var item in e.NewItems)
		{
			var button = CreateCategoryButton((ICategorySelection)item);
			stackPanelCategory.Children.Insert(index++, button);
		}
	}

	private void CategoryButton_Selected(object? sender, RoutedEventArgs e)
	{
		if (sender is CategoryButton button
			&& button.Content is ICategorySelection selection)
		{
			SelectedItem?.OnDeselected();
			SelectedItem = selection;
			SelectedItem.OnSelected();
		}
	}

	private void RemoveItem(NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems is null
			|| e.OldStartingIndex < 0)
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
		if (e.OldItems is null
			|| e.OldStartingIndex < 0
			|| e.NewItems is null
			|| e.NewStartingIndex < 0)
		{
			return;
		}

		for (int i = 0; i < e.OldItems.Count; i++)
		{
			stackPanelCategory.Children[e.OldStartingIndex + i] =
				CreateCategoryButton((ICategorySelection)e.NewItems[i]!);
		}
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
