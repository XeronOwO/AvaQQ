namespace AvaQQ.Core.Entities;

/// <summary>
/// 分类
/// </summary>
public enum Category
{
	/// <summary>
	/// 用户
	/// </summary>
	User,
	/// <summary>
	/// 群
	/// </summary>
	Group,
}

/// <summary>
/// 分类扩展
/// </summary>
public static class CategoryExtensions
{
	/// <summary>
	/// 获取分类名称
	/// </summary>
	/// <param name="category">分类</param>
	/// <returns>分类名称</returns>
	public static string GetName(this Category category)
	{
		return category switch
		{
			Category.User => "user",
			Category.Group => "group",
			_ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
		};
	}
}
