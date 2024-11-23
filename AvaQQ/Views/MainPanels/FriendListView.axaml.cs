using Avalonia;
using Avalonia.Controls;
using AvaQQ.SDK;
using AvaQQ.ViewModels.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace AvaQQ.Views.MainPanels;

public partial class FriendListView : UserControl
{
	public FriendListView()
	{
		_entries = [];
		_entries.CollectionChanged += Entries_CollectionChanged;

		InitializeComponent();

		_ = FirstLoadFriendListAsync();
	}

	private readonly ObservableCollection<EntryView> _entries;

	private async Task FirstLoadFriendListAsync()
	{
		if (Application.Current is not AppBase app
			|| app.Adapter is not { } adapter)
		{
			return;
		}

		try
		{
			var avatarManager = app.ServiceProvider.GetRequiredService<IAvatarManager>();
			var friends = (await adapter.GetFriendListAsync()).ToArray();
			for (int i = 0; i < friends.Length; i++)
			{
				var friend = friends[i];

				var title = friend.Nickname;
				if (!string.IsNullOrEmpty(friend.Remark))
				{
					title += $"({friend.Remark})";
				}
				var icon = avatarManager.GetUserAvatarAsync(friend.Uin, 40);

				EntryViewModel model;
				if (i < _entries.Count)
				{
					model = (EntryViewModel)_entries[i].DataContext!;
				}
				else
				{
					model = new EntryViewModel();
					_entries.Add(new EntryView()
					{
						DataContext = model,
					});
				}

				model.Title = title;
				model.Icon = icon;
			}

			for (int i = friends.Length; i < _entries.Count; i++)
			{
				_entries.RemoveAt(i);
			}
		}
		catch (Exception e)
		{
			app.ServiceProvider.GetRequiredService<ILogger<FriendListView>>()
				.LogError(e, "Failed to update friend list.");
		}
	}

	private void Entries_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		switch (e.Action)
		{
			case NotifyCollectionChangedAction.Add:
				AddEntry(e);
				break;
			case NotifyCollectionChangedAction.Remove:
				RemoveEntry(e);
				break;
			case NotifyCollectionChangedAction.Replace:
				ReplaceEntry(e);
				break;
			case NotifyCollectionChangedAction.Move:
				MoveEntry(e);
				break;
			case NotifyCollectionChangedAction.Reset:
				break;
			default:
				break;
		}
	}

	private void AddEntry(NotifyCollectionChangedEventArgs e)
	{
		if (e.NewItems is null
			|| e.NewStartingIndex < 0)
		{
			return;
		}

		var index = e.NewStartingIndex;
		foreach (var item in e.NewItems)
		{
			stackPanelFriends.Children.Insert(index++, (EntryView)item);
		}
	}

	private void RemoveEntry(NotifyCollectionChangedEventArgs e)
	{
		if (e.OldItems is null
			|| e.OldStartingIndex < 0)
		{
			return;
		}

		for (int i = 0; i < e.OldItems.Count; i++)
		{
			stackPanelFriends.Children.RemoveAt(e.OldStartingIndex);
		}
	}

	private void ReplaceEntry(NotifyCollectionChangedEventArgs e)
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
			stackPanelFriends.Children[e.OldStartingIndex + i] = (EntryView)e.NewItems[i]!;
		}
	}

	private void MoveEntry(NotifyCollectionChangedEventArgs e)
	{
		throw new NotImplementedException();
	}
}