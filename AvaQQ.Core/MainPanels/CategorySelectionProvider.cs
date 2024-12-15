using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AvaQQ.Core.MainPanels;

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
			try
			{
				selections.Add((ICategorySelection)scopedServiceProvider.GetRequiredService(type));
			}
			catch (Exception e)
			{
				_logger.LogWarning(e, "Failed to create category selection from type {Type}.", type);
			}
		}
		return selections;
	}
}
