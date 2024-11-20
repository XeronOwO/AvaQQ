using Avalonia.Controls;
using AvaQQ.SDK.Adapters;

namespace Onebot11ForwardWebSocketAdapter;

internal class AdapterSelection : IAdapterSelection
{
	public UserControl? UserControl
		=> new AdapterSelectionView()
		{
			DataContext = new AdapterSelectionViewModel(),
		};

	public override string ToString()
		=> SR.TextOnebot11ForwardWebSocketAdapter;
}
