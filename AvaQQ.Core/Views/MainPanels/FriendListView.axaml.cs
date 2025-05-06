using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using AvaQQ.Core.Caches;
using AvaQQ.Core.Events;
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
	private readonly IUserCache _userCache;

	private readonly IAvatarCache _avatarCache;

	private readonly EventStation _events;

	/// <summary>
	/// 创建好友列表视图
	/// </summary>
	public FriendListView(
		IUserCache userCache,
		IAvatarCache avatarCache,
		EventStation events
		)
	{
		CirculationInjectionDetector<FriendListView>.Enter();

		_userCache = userCache;
		_avatarCache = avatarCache;
		_events = events;

		DataContext = new FriendListViewModel();
		InitializeComponent();

		Loaded += FriendListView_Loaded;
		scrollViewer.PropertyChanged += ScrollViewer_PropertyChanged;
		textBoxFilter.TextChanged += TextBoxFilter_TextChanged;

		_entryViewHeightLazy = new(() =>
		{
			stackPanel.Children.Add(_testEntryView);
			_testEntryView.Measure(Size.Infinity);
			var desiredSize = _testEntryView.DesiredSize;
			stackPanel.Children.Remove(_testEntryView);

			return desiredSize.Height;
		});

		CirculationInjectionDetector<FriendListView>.Leave();

		_events.UserAvatar.Subscribe(OnUserAvatar);
		_events.CachedGetAllFriends.Subscribe(OnCachedGetAllFriends);
	}

	/// <summary>
	/// 析构函数
	/// </summary>
	~FriendListView()
	{
		_events.UserAvatar.Subscribe(OnUserAvatar);
		_events.CachedGetAllFriends.Subscribe(OnCachedGetAllFriends);
	}

	/// <summary>
	/// 创建好友列表视图
	/// </summary>
	public FriendListView() : this(
		DesignerServiceProviderHelper.Root.GetRequiredService<IUserCache>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<IAvatarCache>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<EventStation>()
		)
	{
	}

	private void FriendListView_Loaded(object? sender, RoutedEventArgs e)
	{
		UpdateFriends(_userCache.GetUsers(v => v.HasLocalData));
	}

	#region 测量

	private static readonly EntryView _testEntryView = new()
	{
		DataContext = new EntryViewModel()
	};

	private readonly Lazy<double> _entryViewHeightLazy;

	private double EntryViewHeight => _entryViewHeightLazy.Value;

	private double ScrollViewerHeight => scrollViewer.Bounds.Height;

	#endregion

	#region 好友

	private readonly List<CachedUserInfo> _friends = [];

	private void UpdateFriends(CachedUserInfo[] users)
	{
		_friends.Clear();
		_friends.AddRange(users);

		UpdateFilteredFriends();
	}

	#endregion

	#region 筛选

	private string? Filter => textBoxFilter.Text;

	private readonly List<CachedUserInfo> _filteredFriends = [];

	private static bool FilterFriend(CachedUserInfo friend, string filter)
	{
		if (friend.Nickname.Contains(filter))
		{
			return true;
		}
		if (friend.Remark is { } remark && remark.Contains(filter))
		{
			return true;
		}
		if (friend.Uin.ToString().Contains(filter))
		{
			return true;
		}

		return false;
	}

	private void UpdateFilteredFriends()
	{
		_filteredFriends.Clear();

		var filter = Filter;
		if (string.IsNullOrEmpty(filter))
		{
			_filteredFriends.AddRange(_friends);
		}
		else
		{
			foreach (var friend in _friends)
			{
				if (FilterFriend(friend, filter))
				{
					_filteredFriends.Add(friend);
				}
			}
		}

		UpdateDisplayedEntries();
	}

	#endregion

	#region 显示

	private readonly List<EntryView> _displayedEntries = [];

	private int DisplayedEntryCount
	{
		get
		{
			if (EntryViewHeight == 0)
			{
				return 0;
			}

			return Math.Min(
				(int)Math.Ceiling(ScrollViewerHeight / EntryViewHeight) + 1,
				_filteredFriends.Count
			);
		}
	}

	private void UpdateGrids()
	{
		var count = _filteredFriends.Count;

		while (grid.RowDefinitions.Count < count)
		{
			grid.RowDefinitions.Add(new(new GridLength(EntryViewHeight)));
		}

		while (grid.RowDefinitions.Count > count)
		{
			grid.RowDefinitions.RemoveAt(grid.RowDefinitions.Count - 1);
		}
	}

	private void EnsureEnoughDisplayedEntries()
	{
		var count = DisplayedEntryCount;

		while (_displayedEntries.Count < count)
		{
			var view = new EntryView()
			{
				DataContext = new EntryViewModel()
			};
			grid.Children.Add(view);
			_displayedEntries.Add(view);
		}

		while (_displayedEntries.Count > count)
		{
			var entry = _displayedEntries[^1];
			grid.Children.Remove(entry);
			_displayedEntries.Remove(entry);
		}
	}

	private int DisplayedEntryStart
		=> (int)Math.Floor(scrollViewer.Offset.Y / EntryViewHeight);

	private void UpdateDisplayedEntries()
	{
		UpdateGrids();
		EnsureEnoughDisplayedEntries();

		var start = DisplayedEntryStart;
		var count = DisplayedEntryCount;

		for (int i = 0; i < count; i++)
		{
			var friendIndex = start + i;
			if (friendIndex >= _filteredFriends.Count)
			{
				continue;
			}
			var friend = _filteredFriends[friendIndex];
			var entry = _displayedEntries[i];
			var model = (EntryViewModel)entry.DataContext!;
			model.Icon = _avatarCache.GetUserAvatar(friend.Uin, 40);
			model.Title = friend.Remark ?? friend.Nickname;
			Grid.SetRow(entry, friendIndex);
		}
	}

	#endregion

	#region 事件处理

	private void TextBoxFilter_TextChanged(object? sender, TextChangedEventArgs e)
	{
		UpdateFilteredFriends();
	}

	private void ScrollViewer_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Property == ScrollViewer.OffsetProperty)
		{
			UpdateDisplayedEntries();
		}
		else if (e.Property == BoundsProperty)
		{
			UpdateDisplayedEntries();
		}
	}

	private void OnUserAvatar(object? sender, BusEventArgs<AvatarCacheId, Bitmap?> e)
	{
		Dispatcher.UIThread.Invoke(() =>
		{
			var start = DisplayedEntryStart;
			var count = DisplayedEntryCount;

			for (int i = 0; i < count; i++)
			{
				if (start + i >= count)
				{
					continue;
				}

				var user = _filteredFriends[start + i];
				if (user.Uin != e.Id.Uin)
				{
					continue;
				}

				var dataContext = (EntryViewModel)_displayedEntries[i].DataContext!;
				dataContext.Icon = e.Result;
			}
		});
	}

	private void OnCachedGetAllFriends(object? sender, BusEventArgs<CommonEventId, CachedUserInfo[]> e)
	{
		Dispatcher.UIThread.Invoke(() =>
		{
			UpdateFriends(_userCache.GetUsers(v => v.HasLocalData));
		});
	}

	#endregion
}
