using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace AvaQQ.SDK.MainPanels;

/// <summary>
/// 分类选项提供者
/// </summary>
public interface ICategorySelectionProvider
{
	List<ICategorySelection> CreateSelections();

	/// <summary>
	/// 注册分类选项<br/>
	/// 请自行完成 <see cref="IServiceCollection"/> 的注册，后续会通过依赖注入获取该服务。
	/// </summary>
	/// <typeparam name="T">选项类型</typeparam>
	void Register<T>()
		where T : ICategorySelection;

	/// <summary>
	/// 注册分类选项<br/>
	/// 请自行完成 <see cref="IServiceCollection"/> 的注册，后续会通过依赖注入获取该服务。
	/// </summary>
	/// <param name="type">选项类型</param>
	void Register(Type type);
}
