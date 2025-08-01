namespace AvaQQ.SDK.Entities;

public record struct AvatarId(Category Category, ulong Uin, uint Size)
{
	public readonly string Url
		=> Category switch
		{
			Category.User => $"https://q1.qlogo.cn/g?b=qq&nk={Uin}&s={Size}",
			Category.Group => $"https://p.qlogo.cn/gh/{Uin}/{Uin}/{Size}",
			_ => throw new NotSupportedException($"Unsupported category: {Category}")
		};

	public static AvatarId User(ulong userUin, uint size = 40)
		=> new(Category.User, userUin, size);

	public static AvatarId Group(ulong groupUin, uint size = 40)
		=> new(Category.Group, groupUin, size);
}
