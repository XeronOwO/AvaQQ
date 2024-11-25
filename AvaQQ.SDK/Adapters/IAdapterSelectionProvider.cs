using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace AvaQQ.SDK.Adapters;

/// <summary>
/// 协议适配器提供者，用于注册、获取适配器。
/// </summary>
public interface IAdapterSelectionProvider
{
	/// <summary>
	/// 创建适配器选项
	/// </summary>
	List<IAdapterSelection> CreateSelections(IServiceProvider scopedServiceProvider);

	/// <summary>
	/// 注册适配器选项<br/>
	/// 请自行完成 <see cref="IServiceCollection"/> 的注册，推荐注册为 Scoped，后续会通过依赖注入获取该服务。
	/// </summary>
	/// <typeparam name="T">选项类型</typeparam>
	void Register<T>()
		where T : IAdapterSelection;

	/// <summary>
	/// 注册适配器选项<br/>
	/// 请自行完成 <see cref="IServiceCollection"/> 的注册，推荐注册为 Scoped，后续会通过依赖注入获取该服务。
	/// </summary>
	/// <param name="type">选项类型</param>
	void Register(Type type);
}
