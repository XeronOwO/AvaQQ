namespace AvaQQ.Core.Entities;

/// <summary>
/// 头像缓存 ID
/// </summary>
/// <param name="Uin">QQ 号或群号</param>
/// <param name="Size">头像尺寸</param>
/// <param name="Category">分类</param>
public record struct AvatarId(Category Category, ulong Uin, uint Size)
{
	/// <summary>
	/// 获取头像的 URL
	/// </summary>
	public readonly string Url
		=> Category switch
		{
			Category.User => $"https://q1.qlogo.cn/g?b=qq&nk={Uin}&s={Size}",
			Category.Group => $"https://p.qlogo.cn/gh/{Uin}/{Uin}/{Size}",
			_ => throw new NotSupportedException($"Unsupported category: {Category}")
		};
}

/// <summary>
/// Uin ID
/// </summary>
/// <param name="Uin">QQ 号或群号</param>
public record struct UinId(ulong Uin);
