using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.Core.Adapters;
using AvaQQ.Core.Caches;
using AvaQQ.Core.Utils;
using AvaQQ.Core.ViewModels.MainPanels;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ.Core.Views.MainPanels;

/// <summary>
/// 好友列表视图
/// </summary>
public partial class FriendListView : UserControl
{
	private readonly IAvatarCache _avatarManager;

	private readonly IFriendCache _friendManager;

	/// <summary>
	/// 创建好友列表视图
	/// </summary>
	public FriendListView(IAvatarCache avatarCache, IFriendCache friendCache)
	{
		CirculationInjectionDetector<FriendListView>.Enter();

		DataContext = new FriendListViewModel();
		InitializeComponent();

		Loaded += FriendListView_Loaded;
		scrollViewer.PropertyChanged += ScrollViewer_PropertyChanged;
		textBoxFilter.TextChanged += TextBoxFilter_TextChanged;
		_avatarManager = avatarCache;
		_friendManager = friendCache;

		CirculationInjectionDetector<FriendListView>.Leave();
	}

	/// <summary>
	/// 创建好友列表视图
	/// </summary>
	public FriendListView() : this(
		DesignerServiceProviderHelper.Root.GetRequiredService<IAvatarCache>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<IFriendCache>()
		)
	{
	}

	private async void FriendListView_Loaded(object? sender, RoutedEventArgs e)
	{
		CalculateEntryViewHeight();
		CalculateScrollViewerHeight();
		await UpdateFriendInfoListAsync();
		UpdateFilteredFriendList();
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
	private readonly List<FriendInfo> _friends = [];

	/// <summary>
	/// 经过筛选后的好友列表
	/// </summary>
	private readonly List<FriendInfo> _filteredFriends = [];

	private int _displayedEntryCount;

	private void UpdateDisplayedEntryList()
	{
		if (_entryViewHeight == 0)
		{
			return;
		}

		_displayedEntryCount = Math.Min(
			(int)Math.Ceiling(_scrollViewerHeight / _entryViewHeight) + 1,
			_filteredFriends.Count
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

	private async Task UpdateFriendInfoListAsync()
	{
		_friends.Clear();
		var friends = await _friendManager.GetAllFriendInfosAsync();
		//for (int i = 0; i < 1000; i++) // 压力测试
		_friends.AddRange(friends);
	}

	private void UpdateFilteredFriendList()
	{
		_filteredFriends.Clear();

		var filter = textBoxFilter.Text;
		if (string.IsNullOrEmpty(filter))
		{
			_filteredFriends.AddRange(_friends);
			return;
		}

		foreach (var friend in _friends)
		{
			if (friend.Uin.ToString().Contains(filter)
				|| friend.Nickname.Contains(filter)
				|| friend.Remark.Contains(filter))
			{
				_filteredFriends.Add(friend);
			}
		}
	}

	private void UpdateGrid()
	{
		var height = new GridLength(_entryViewHeight);

		for (int i = 0; i < Math.Min(_filteredFriends.Count, grid.RowDefinitions.Count); i++)
		{
			grid.RowDefinitions[i].Height = height;
		}

		while (grid.RowDefinitions.Count != _filteredFriends.Count)
		{
			if (grid.RowDefinitions.Count < _filteredFriends.Count)
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

		for (int i = 0; i < _displayedEntries.Count; i++)
		{
			var entry = _displayedEntries[i];
			var friendIndex = newIndex + i;
			if (friendIndex >= _filteredFriends.Count
				|| entry.DataContext is not EntryViewModel model)
			{
				continue;
			}

			var friend = _filteredFriends[friendIndex];
			if (model.Id == friend.Uin)
			{
				continue;
			}

			model.Id = friend.Uin;
			model.Icon = _avatarManager.GetUserAvatarAsync(friend.Uin, 40);
			model.Title = Task.FromResult(string.IsNullOrEmpty(friend.Remark)
				? friend.Nickname
				: $"{friend.Remark} ({friend.Nickname})");

			Grid.SetRow(entry, friendIndex);
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
			UpdateFilteredFriendList();
			UpdateDisplayedEntryList();
			UpdateGrid();
			UpdateEntryContent();
		}
	}

	private void TextBoxFilter_TextChanged(object? sender, TextChangedEventArgs e)
	{
		UpdateFilteredFriendList();
		UpdateDisplayedEntryList();
		UpdateGrid();
		UpdateEntryContent();
	}
}
