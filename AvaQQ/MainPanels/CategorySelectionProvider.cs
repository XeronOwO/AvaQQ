using AvaQQ.SDK.MainPanels;
using System.Collections.Generic;

namespace AvaQQ.MainPanels;

internal class CategorySelectionProvider : List<ICategorySelection>, ICategorySelectionProvider
{
	public CategorySelectionProvider()
	{
		Add(new RecentCategorySelection());
		Add(new FriendCategorySelection());
		Add(new GroupCategorySelection());
	}
}
