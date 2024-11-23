using AvaQQ.SDK.MainPanels;
using System;
using System.Collections.Generic;

namespace AvaQQ.MainPanels;

internal class CategorySelectionProvider : List<ICategorySelection>, ICategorySelectionProvider
{
	public CategorySelectionProvider(IServiceProvider serviceProvider)
	{
		Add(new RecentCategorySelection());
		Add(new FriendCategorySelection(serviceProvider));
		Add(new GroupCategorySelection());
	}
}
