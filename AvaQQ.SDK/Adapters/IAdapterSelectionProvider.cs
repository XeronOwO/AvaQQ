using System.Collections.Generic;

namespace AvaQQ.SDK.Adapters;

/// <summary>
/// 协议适配器提供者，用于注册、获取适配器。
/// </summary>
public interface IAdapterSelectionProvider : IList<IAdapterSelection>
{
}
