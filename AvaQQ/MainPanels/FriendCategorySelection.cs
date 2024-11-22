using Avalonia.Controls;
using AvaQQ.Resources;
using AvaQQ.SDK.MainPanels;
using AvaQQ.ViewModels.MainPanels;
using AvaQQ.Views.MainPanels;

namespace AvaQQ.MainPanels;

internal class FriendCategorySelection : ICategorySelection
{
	public UserControl? UserControl => new FriendListView()
	{
		DataContext = new FriendListViewModel(),
	};

	public override string ToString()
	{
		return SR.TextFriend;
	}
}
