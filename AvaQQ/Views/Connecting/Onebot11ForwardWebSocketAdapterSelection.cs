using Avalonia.Controls;
using AvaQQ.Resources;

namespace AvaQQ.Views.Connecting;

internal class Onebot11ForwardWebSocketAdapterSelection : AdapterSelection
{
	public override UserControl? UserControl => new Onebot11ForwardWebSocketAdapterOptionsView();

	public override string ToString()
	{
		return SR.TextOnebot11ForwardWebSocketAdapter;
	}
}
