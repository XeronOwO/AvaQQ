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
	/// 当适配器改变时触发
	/// </summary>
	event EventHandler<AdapterChangedEventArgs>? OnAdapterChanged;
}
