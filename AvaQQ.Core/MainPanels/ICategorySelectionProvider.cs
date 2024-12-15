using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ.Core.MainPanels;

/// <summary>
/// 分类选项提供者
/// </summary>
public interface ICategorySelectionProvider
{
	/// <summary>
	/// 创建分类选项
	/// </summary>
	List<ICategorySelection> CreateSelections(IServiceProvider scopedServiceProvider);

	/// <summary>
	/// 注册分类选项<br/>
	/// 请自行完成 <see cref="IServiceCollection"/> 的注册，推荐注册为 Scoped，后续会通过依赖注入获取该服务。
	/// </summary>
	/// <typeparam name="T">选项类型</typeparam>
	void Register<T>()
		where T : ICategorySelection;

	/// <summary>
	/// 注册分类选项<br/>
	/// 请自行完成 <see cref="IServiceCollection"/> 的注册，推荐注册为 Scoped，后续会通过依赖注入获取该服务。
	/// </summary>
	/// <param name="type">选项类型</param>
	void Register(Type type);
}
