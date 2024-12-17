namespace AvaQQ.Core.Adapters;

/// <summary>
/// 适配器变更事件参数
/// </summary>
public class AdapterChangedEventArgs(IAdapter? old, IAdapter? @new) : EventArgs
{
	/// <summary>
	/// 旧适配器
	/// </summary>
	public IAdapter? Old => old;

	/// <summary>
	/// 新适配器
	/// </summary>
	public IAdapter? New => @new;
}
