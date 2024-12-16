using Avalonia.Controls;
using AvaQQ.Core.Adapters;

namespace AvaQQ.Adapters.Onebot11ForwardWebSocket;

internal class AdapterSelection(AdapterSelectionView view) : IAdapterSelection
{
	public UserControl? View => view;

	public string Id => nameof(Onebot11ForwardWebSocket);

	public void OnSelected()
	{

	}

	public void OnDeselected()
	{

	}

	public void Dispose()
	{

	}

	public override string ToString()
		=> SR.TextOnebot11ForwardWebSocketAdapter;
}
