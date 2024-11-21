using Avalonia.Controls;
using AvaQQ.Resources;
using AvaQQ.SDK.MainPanels;

namespace AvaQQ.MainPanels;

internal class GroupCategorySelection : ICategorySelection
{
	public UserControl? UserControl => null;

	public override string ToString()
	{
		return SR.TextGroup;
	}
}
