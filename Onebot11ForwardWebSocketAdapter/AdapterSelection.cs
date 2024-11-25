using Avalonia.Controls;
using AvaQQ.SDK.Adapters;

namespace Onebot11ForwardWebSocketAdapter;

internal class AdapterSelection : IAdapterSelection, IDisposable
{
	public UserControl? UserControl
		=> new AdapterSelectionView()
		{
			DataContext = new AdapterSelectionViewModel(),
		};

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
