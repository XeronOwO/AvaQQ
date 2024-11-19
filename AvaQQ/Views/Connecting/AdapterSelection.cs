using Avalonia.Controls;

namespace AvaQQ.Views.Connecting;

internal abstract class AdapterSelection
{
	public virtual UserControl? UserControl => null;

	public abstract override string ToString();
}
