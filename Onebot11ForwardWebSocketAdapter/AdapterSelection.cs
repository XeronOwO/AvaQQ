using Avalonia.Controls;
using AvaQQ.Core.Adapters;

namespace Onebot11ForwardWebSocketAdapter;

internal class AdapterSelection(AdapterSelectionView view) : IAdapterSelection
{
	public UserControl? View => view;

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
