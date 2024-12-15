using System.Diagnostics.CodeAnalysis;

namespace AvaQQ.SDK;

/// <summary>
/// 设计器服务提供器助手<br/>
/// 由于设计器无法使用微软的依赖注入（会导致崩溃），所以采用手动提供的方式，不推荐在正式代码中使用
/// </summary>
public static class DesignerServiceProviderHelper
{
	private static IServiceProvider? _root;

	/// <summary>
	/// 根服务提供器，仅用于防止设计器崩溃，不推荐在正式代码中使用
	/// </summary>
	[NotNull]
	public static IServiceProvider? Root
	{
		get => _root ?? throw new Exception($"{nameof(Root)} not initialized.");
		set => _root = value!;
	}
}
