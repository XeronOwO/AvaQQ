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

public partial class FriendListView : UserControl
{
	public FriendListView()
	{
		InitializeComponent();

		Loaded += FriendListView_Loaded;
		scrollViewer.PropertyChanged += ScrollViewer_PropertyChanged;
	}

	private bool _displayedEntriesInitialized;

	private async void FriendListView_Loaded(object? sender, RoutedEventArgs e)
	{
		CalculateEntryViewHeight();
		CalculateScrollViewerHeight();
		UpdateDisplayedEntryList();
		await UpdateFriendInfoListAsync();
		UpdateGrid();
		UpdateEntryContent();
		_displayedEntriesInitialized = true;
	}

	private readonly List<EntryView> _displayedEntries = [];

	private static EntryView CreateEntryView()
		=> new()
		{
			DataContext = new EntryViewModel()
		};

	private double _entryViewHeight;

	private void CalculateEntryViewHeight()
	{
		if (_displayedEntries.Count == 0)
		{
			_displayedEntries.Add(CreateEntryView());
		}

		var view = _displayedEntries[0];
		stackPanel.Children.Add(view);
		view.Measure(Size.Infinity);
		var desiredSize = view.DesiredSize;
		stackPanel.Children.Remove(view);

		_entryViewHeight = desiredSize.Height;
	}

	private double _scrollViewerHeight;

	private void CalculateScrollViewerHeight()
	{
		_scrollViewerHeight = scrollViewer.Bounds.Height;
	}

	private int _displayedEntryCount;

	private void UpdateDisplayedEntryList()
	{
		_displayedEntryCount = (int)Math.Ceiling(_scrollViewerHeight / _entryViewHeight) + 1;
		while (_displayedEntries.Count != _displayedEntryCount)
		{
			if (_displayedEntries.Count < _displayedEntryCount)
			{
				_displayedEntries.Add(CreateEntryView());
			}
			else
			{
				_displayedEntries.RemoveAt(_displayedEntries.Count - 1);
			}
		}
	}

	private readonly List<BriefFriendInfo> _friends = [];

	private async Task UpdateFriendInfoListAsync()
	{
		if (Application.Current is not AppBase app
			|| app.Adapter is not { } adapter)
		{
			return;
		}

		var friends = await adapter.GetFriendListAsync();
		//for (int i = 0; i < 1000; i++) // Ñ¹Á¦²âÊÔ
		//{
		_friends.AddRange(friends);
		//}
	}

	private void UpdateGrid()
	{
		var height = new GridLength(_entryViewHeight);

		for (int i = 0; i < Math.Min(_friends.Count, grid.RowDefinitions.Count); i++)
		{
			grid.RowDefinitions[i].Height = height;
		}

		while (grid.RowDefinitions.Count != _friends.Count)
		{
			if (grid.RowDefinitions.Count < _friends.Count)
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

		if (Application.Current is AppBase app)
		{
			var avatarManager = app.ServiceProvider.GetRequiredService<IAvatarManager>();

			for (int i = 0; i < _displayedEntries.Count; i++)
			{
				var entry = _displayedEntries[i];
				var friendIndex = newIndex + i;
				if (friendIndex >= _friends.Count
					|| entry.DataContext is not EntryViewModel model)
				{
					continue;
				}

				var friend = _friends[friendIndex];
				if (model.Id is int id && id == friend.Uin)
				{
					continue;
				}

				model.Id = friend.Uin;
				model.Icon = avatarManager.GetUserAvatarAsync(friend.Uin, 40);
				model.Title = friend.Nickname;
				if (!string.IsNullOrEmpty(friend.Remark))
				{
					model.Title += $" ({friend.Remark})";
				}

				if (entry.Parent is null)
				{
					grid.Children.Add(entry);
				}
				Grid.SetRow(entry, friendIndex);
			}
		}

		_oldOffset = newOffset;
	}

	private void ScrollViewer_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == ScrollViewer.OffsetProperty)
		{
			UpdateEntryContent();
		}
		else if (e.Property == BoundsProperty
			&& _displayedEntriesInitialized)
		{
			CalculateScrollViewerHeight();
			UpdateDisplayedEntryList();
			UpdateGrid();
			UpdateEntryContent();
		}
	}
}
