using Avalonia.Controls;

namespace AvaQQ.SDK;

public abstract class ConnectWindowBase : Window
{
	public abstract void BeginConnect();

	public abstract void EndConnect(IAdapter? adapter);
}
