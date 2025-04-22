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
/// 群聊列表视图
/// </summary>
public partial class GroupListView : UserControl
{
	private readonly IGroupCache _groupCache;

	private readonly IAvatarCache _avatarCache;

	private readonly EventStation _events;

	/// <summary>
	/// 创建群聊列表视图
	/// </summary>
	public GroupListView(
		IGroupCache groupCache,
		IAvatarCache avatarCache,
		EventStation events
		)
	{
		CirculationInjectionDetector<GroupListView>.Enter();

		_groupCache = groupCache;
		_avatarCache = avatarCache;
		_events = events;

		DataContext = new GroupListViewModel();
		InitializeComponent();

		Loaded += GroupListView_Loaded;
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

		CirculationInjectionDetector<GroupListView>.Leave();

		_events.GroupAvatar.OnDone += OnGroupAvatar;
		_events.CachedGetAllJoinedGroups.OnDone += OnCachedGetAllJoinedGroups;
	}

	/// <summary>
	/// 析构函数
	/// </summary>
	~GroupListView()
	{
		_events.GroupAvatar.OnDone -= OnGroupAvatar;
		_events.CachedGetAllJoinedGroups.OnDone -= OnCachedGetAllJoinedGroups;
	}

	/// <summary>
	/// 创建群聊列表视图
	/// </summary>
	public GroupListView() : this(
		DesignerServiceProviderHelper.Root.GetRequiredService<IGroupCache>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<IAvatarCache>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<EventStation>()
		)
	{
	}

	private void GroupListView_Loaded(object? sender, RoutedEventArgs e)
	{
		UpdateGroups(_groupCache.GetGroups(v => v.HasLocalData));
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

	#region 群

	private readonly List<CachedGroupInfo> _groups = [];

	private void UpdateGroups(CachedGroupInfo[] groups)
	{
		_groups.Clear();
		_groups.AddRange(groups);

		UpdateFilteredGroups();
	}

	#endregion

	#region 筛选

	private string? Filter => textBoxFilter.Text;

	private readonly List<CachedGroupInfo> _filteredGroups = [];

	private static bool FilterGroup(CachedGroupInfo group, string filter)
	{
		if (group.Name.Contains(filter))
		{
			return true;
		}
		if (group.Remark is { } remark && remark.Contains(filter))
		{
			return true;
		}
		if (group.Uin.ToString().Contains(filter))
		{
			return true;
		}

		return false;
	}

	private void UpdateFilteredGroups()
	{
		_filteredGroups.Clear();

		var filter = Filter;
		if (string.IsNullOrEmpty(filter))
		{
			_filteredGroups.AddRange(_groups);
		}
		else
		{
			foreach (var group in _groups)
			{
				if (FilterGroup(group, filter))
				{
					_filteredGroups.Add(group);
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
				_filteredGroups.Count
			);
		}
	}

	private void UpdateGrids()
	{
		var count = _filteredGroups.Count;

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
			var groupIndex = start + i;
			if (groupIndex >= _filteredGroups.Count)
			{
				continue;
			}
			var group = _filteredGroups[groupIndex];
			var entry = _displayedEntries[i];
			var model = (EntryViewModel)entry.DataContext!;
			model.Icon = _avatarCache.GetGroupAvatar(group.Uin, 40);
			model.Title = group.Remark ?? group.Name;
			Grid.SetRow(entry, groupIndex);
		}
	}

	#endregion

	#region 事件处理

	private void TextBoxFilter_TextChanged(object? sender, TextChangedEventArgs e)
	{
		UpdateFilteredGroups();
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

	private void OnGroupAvatar(object? sender, BusEventArgs<AvatarCacheId, Bitmap?> e)
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

				var group = _filteredGroups[start + i];
				if (group.Uin != e.Id.Uin)
				{
					continue;
				}

				var dataContext = (EntryViewModel)_displayedEntries[i].DataContext!;
				dataContext.Icon = e.Result;
			}
		});
	}

	private void OnCachedGetAllJoinedGroups(object? sender, BusEventArgs<CommonEventId> e)
	{
		Dispatcher.UIThread.Invoke(() =>
		{
			UpdateGroups(_groupCache.GetGroups(v => v.HasLocalData));
		});
	}

	#endregion
}