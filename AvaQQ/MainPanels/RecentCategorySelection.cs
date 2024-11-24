using Avalonia.Controls;
using AvaQQ.Resources;
using AvaQQ.SDK.MainPanels;

namespace AvaQQ.MainPanels;

internal class RecentCategorySelection : ICategorySelection
{
	public UserControl? UserControl => null;

	public void OnDeselected()
	{

	}

	public void OnSelected()
	{

	}

	public override string ToString()
	{
		return SR.TextRecent;
	}
}
