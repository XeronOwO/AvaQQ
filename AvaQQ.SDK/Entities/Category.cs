namespace AvaQQ.SDK.Entities;

public enum Category
{
	User,
	Group,
}
public static class CategoryExtensions
{
	public static string GetLowercaseName(this Category category)
	{
		return category switch
		{
			Category.User => "user",
			Category.Group => "group",
			_ => throw new ArgumentOutOfRangeException(nameof(category))
		};
	}
}
