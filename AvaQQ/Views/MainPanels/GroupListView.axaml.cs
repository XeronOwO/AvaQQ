using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using AvaQQ.ViewModels.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaQQ.Views.MainPanels;

public partial class GroupListView : UserControl
{
	public GroupListView()
	{
		InitializeComponent();

		Loaded += GroupListView_Loaded;
		scrollViewer.PropertyChanged += ScrollViewer_PropertyChanged;
		textBoxFilter.TextChanged += TextBoxFilter_TextChanged;
	}

	private async void GroupListView_Loaded(object? sender, RoutedEventArgs e)
	{
		CalculateEntryViewHeight();
		CalculateScrollViewerHeight();
		await UpdateGroupInfoListAsync();
		UpdateFilteredGroupList();
		UpdateDisplayedEntryList();
		UpdateGrid();
		UpdateEntryContent();
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

	/// <summary>
	/// 原始的好友列表
	/// </summary>
	private readonly List<BriefGroupInfo> _groups = [];

	/// <summary>
	/// 经过筛选后的好友列表
	/// </summary>
	private readonly List<BriefGroupInfo> _filteredGroups = [];

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
		var app = AppBase.Current;
		if (app.Adapter is not { } adapter)
		{
			return;
		}

		_groups.Clear();
		var groups = await adapter.GetGroupListAsync();
		//for (int i = 0; i < 1000; i++) // 压力测试
		_groups.AddRange(groups);
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

	private void UpdateEntryContent()
	{
		var newOffset = scrollViewer.Offset.Y;

		var oldIndex = (int)Math.Floor(_oldOffset / _entryViewHeight);
		var newIndex = (int)Math.Floor(newOffset / _entryViewHeight);
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

		var app = AppBase.Current;
		var avatarManager = app.ServiceProvider.GetRequiredService<IAvatarManager>();
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
			if (model.Id is ulong id && id == group.Uin)
			{
				continue;
			}

			model.Id = group.Uin;
			model.Icon = avatarManager.GetGroupAvatarAsync(group.Uin, 40);
			model.Title = string.IsNullOrEmpty(group.Remark)
				? group.Name
				: $"{group.Remark} ({group.Name})";

			Grid.SetRow(entry, groupIndex);
		}

		_oldOffset = newOffset;
	}

	private void ScrollViewer_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == ScrollViewer.OffsetProperty)
		{
			UpdateEntryContent();
		}
		else if (e.Property == BoundsProperty)
		{
			CalculateScrollViewerHeight();
			UpdateFilteredGroupList();
			UpdateDisplayedEntryList();
			UpdateGrid();
			UpdateEntryContent();
		}
	}

	private void TextBoxFilter_TextChanged(object? sender, TextChangedEventArgs e)
	{
		UpdateFilteredGroupList();
		UpdateDisplayedEntryList();
		UpdateGrid();
		UpdateEntryContent();
	}
}