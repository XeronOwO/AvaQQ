namespace AvaQQ.Core.Adapters;

/// <summary>
/// 适配器提供者<br/>
/// 用于托管创建的适配器
/// </summary>
public interface IAdapterProvider : IDisposable
{
	/// <summary>
	/// 适配器
	/// </summary>
	IAdapter? Adapter { get; set; }

	/// <summary>
	/// 适配器（确保不为空）
	/// </summary>
	/// <exception cref="InvalidOperationException"/>
	IAdapter EnsuredAdapter => Adapter ?? throw new InvalidOperationException("Adapter is not available.");
}
