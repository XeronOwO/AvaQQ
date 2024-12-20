using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaQQ.Core.Adapters;
using AvaQQ.Core.Caches;
using AvaQQ.Core.Utils;
using AvaQQ.Core.ViewModels.MainPanels;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ.Core.Views.MainPanels;

/// <summary>
/// Ⱥ���б���ͼ
/// </summary>
public partial class GroupListView : UserControl
{
	private readonly IGroupCache _groupCache;

	private readonly IAvatarCache _avatarCache;

	/// <summary>
	/// ����Ⱥ���б���ͼ
	/// </summary>
	public GroupListView(
		IGroupCache groupCache,
		IAvatarCache avatarCache
		)
	{
		CirculationInjectionDetector<GroupListView>.Enter();

		_groupCache = groupCache;
		_avatarCache = avatarCache;

		DataContext = new GroupListViewModel();
		InitializeComponent();

		Loaded += GroupListView_Loaded;
		scrollViewer.PropertyChanged += ScrollViewer_PropertyChanged;
		textBoxFilter.TextChanged += TextBoxFilter_TextChanged;
		groupCache.OnUpdated += GroupCache_OnUpdated;

		CirculationInjectionDetector<GroupListView>.Leave();
	}

	/// <summary>
	/// ����Ⱥ���б���ͼ
	/// </summary>
	public GroupListView() : this(
		DesignerServiceProviderHelper.Root.GetRequiredService<IGroupCache>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<IAvatarCache>()
		)
	{
	}

	private async void GroupListView_Loaded(object? sender, RoutedEventArgs e)
	{
		CalculateEntryViewHeight();
		CalculateScrollViewerHeight();
		await UpdateGroupInfoListAsync();
		UpdateFilteredGroupList();
		UpdateDisplayedEntryList();
		UpdateGrid();
		UpdateDisplayedEntries();
	}

	private readonly List<EntryView> _displayedEntries = [];

	private static EntryView CreateEntryView()
		=> new()
		{
			DataContext = new EntryViewModel()
		};

	private static readonly EntryView _testEntryView = CreateEntryView();

	private double _entryViewHeight;

	private void CalculateEntryViewHeight()
	{
		stackPanel.Children.Add(_testEntryView);
		_testEntryView.Measure(Size.Infinity);
		var desiredSize = _testEntryView.DesiredSize;
		stackPanel.Children.Remove(_testEntryView);

		_entryViewHeight = desiredSize.Height;
	}

	private double _scrollViewerHeight;

	private void CalculateScrollViewerHeight()
	{
		_scrollViewerHeight = scrollViewer.Bounds.Height;
	}

	private GroupInfo[] _groups = [];

	private readonly List<GroupInfo> _filteredGroups = [];

	private int _displayedEntryCount;

	private void UpdateDisplayedEntryList()
	{
		if (_entryViewHeight == 0)
		{
			return;
		}

		_displayedEntryCount = Math.Min(
			(int)Math.Ceiling(_scrollViewerHeight / _entryViewHeight) + 1,
			_filteredGroups.Count
		);

		while (_displayedEntries.Count != _displayedEntryCount)
		{
			if (_displayedEntries.Count < _displayedEntryCount)
			{
				var view = CreateEntryView();
				grid.Children.Add(view);
				_displayedEntries.Add(view);
			}
			else
			{
				var entry = _displayedEntries[^1];
				grid.Children.Remove(entry);
				_displayedEntries.Remove(entry);
			}
		}
	}

	private async Task UpdateGroupInfoListAsync()
	{
		_groups = [.. await _groupCache.GetAllGroupInfosAsync()];
	}

	private void UpdateFilteredGroupList()
	{
		_filteredGroups.Clear();

		var filter = textBoxFilter.Text;
		if (string.IsNullOrEmpty(filter))
		{
			_filteredGroups.AddRange(_groups);
			return;
		}

		foreach (var group in _groups)
		{
			if (group.Uin.ToString().Contains(filter)
				|| group.Name.Contains(filter)
				|| group.Remark.Contains(filter))
			{
				_filteredGroups.Add(group);
			}
		}
	}

	private void UpdateGrid()
	{
		var height = new GridLength(_entryViewHeight);

		for (int i = 0; i < Math.Min(_filteredGroups.Count, grid.RowDefinitions.Count); i++)
		{
			grid.RowDefinitions[i].Height = height;
		}

		while (grid.RowDefinitions.Count != _filteredGroups.Count)
		{
			if (grid.RowDefinitions.Count < _filteredGroups.Count)
			{
				grid.RowDefinitions.Add(new(height));
			}
			else
			{
				grid.RowDefinitions.RemoveAt(grid.RowDefinitions.Count - 1);
			}
		}
	}

	private double _oldOffset;

	private void UpdateDisplayedEntries()
	{
		if (_entryViewHeight <= 0)
		{
			return;
		}

		var newOffset = scrollViewer.Offset.Y;

		var oldIndex = (int)Math.Floor(_oldOffset / _entryViewHeight);
		var newIndex = (int)Math.Floor(newOffset / _entryViewHeight);

		EnsureEnoughDisplayedEntries(oldIndex, newIndex);
		UpdateDisplayedEntryContents(newIndex);

		_oldOffset = newOffset;
	}

	private void EnsureEnoughDisplayedEntries(int oldIndex, int newIndex)
	{
		while (oldIndex != newIndex)
		{
			if (oldIndex < newIndex)
			{
				var entry = _displayedEntries[0];
				_displayedEntries.RemoveAt(0);
				_displayedEntries.Add(entry);
				++oldIndex;
			}
			else
			{
				var entry = _displayedEntries[^1];
				_displayedEntries.RemoveAt(_displayedEntries.Count - 1);
				_displayedEntries.Insert(0, entry);
				--oldIndex;
			}
		}
	}

	private void UpdateDisplayedEntryContents(int newIndex)
	{
		for (int i = 0; i < _displayedEntries.Count; i++)
		{
			var entry = _displayedEntries[i];
			var groupIndex = newIndex + i;
			if (groupIndex >= _filteredGroups.Count
				|| entry.DataContext is not EntryViewModel model)
			{
				continue;
			}

			var group = _filteredGroups[groupIndex];

			model.Icon = _avatarCache.GetGroupAvatarAsync(group.Uin, 40);
			model.Title = _groupCache.GetGroupNameAsync(group.Uin);
			model.Time = _groupCache.GetLatestMessageTime(group.Uin);
			model.Content = _groupCache.GetLatestMessagePreviewAsync(group.Uin);

			Grid.SetRow(entry, groupIndex);
		}
	}

	private void ScrollViewer_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == ScrollViewer.OffsetProperty)
		{
			UpdateDisplayedEntries();
		}
		else if (e.Property == BoundsProperty)
		{
			CalculateScrollViewerHeight();
			UpdateFilteredGroupList();
			UpdateDisplayedEntryList();
			UpdateGrid();
			UpdateDisplayedEntries();
		}
	}

	private void TextBoxFilter_TextChanged(object? sender, TextChangedEventArgs e)
	{
		UpdateFilteredGroupList();
		UpdateDisplayedEntryList();
		UpdateGrid();
		UpdateDisplayedEntries();
	}

	private void GroupCache_OnUpdated(object? sender, EventArgs e)
	{
		Dispatcher.UIThread.Invoke(UpdateDisplayedEntries);
	}
}