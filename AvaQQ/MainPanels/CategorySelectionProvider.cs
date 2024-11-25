using AvaQQ.SDK.MainPanels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace AvaQQ.MainPanels;

internal class CategorySelectionProvider : ICategorySelectionProvider
{
	private readonly List<Type> _categories = [];

	private readonly ILogger<CategorySelectionProvider> _logger;

	public CategorySelectionProvider(IServiceProvider serviceProvider)
	{
		_logger = serviceProvider.GetRequiredService<ILogger<CategorySelectionProvider>>();

		Register<RecentCategorySelection>();
		Register<FriendCategorySelection>();
		Register<GroupCategorySelection>();
	}

	public void Register<T>() where T : ICategorySelection
		=> Register(typeof(T));

	private static readonly Type _categorySelectionType = typeof(ICategorySelection);

	public void Register(Type type)
	{
		try
		{
			if (!type.IsAssignableTo(_categorySelectionType))
			{
				throw new ArgumentException($"Type {type} is not assignable to {_categorySelectionType}.");
			}

			_categories.Add(type);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to register category selection.");
		}
	}

	public List<ICategorySelection> CreateSelections(IServiceProvider scopedServiceProvider)
	{
		var selections = new List<ICategorySelection>();
		foreach (var type in _categories)
		{
			if (scopedServiceProvider.GetService(type) is ICategorySelection selection)
			{
				selections.Add(selection);
			}
			else
			{
				_logger.LogWarning("Failed to create category selection from type {Type}.", type);
			}
		}
		return selections;
	}
}
