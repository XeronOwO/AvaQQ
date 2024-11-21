using Avalonia.Controls;
using AvaQQ.Resources;
using AvaQQ.SDK.MainPanels;

namespace AvaQQ.MainPanels;

internal class FriendCategorySelection : ICategorySelection
{
	public UserControl? UserControl => null;

	public override string ToString()
	{
		return SR.TextFriend;
	}
}
