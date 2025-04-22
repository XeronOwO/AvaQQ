using Avalonia.Controls;
using AvaQQ.Core.Adapters;

namespace AvaQQ.Adapters.Lagrange;

internal class AdapterSelection(AdapterSelectionView view) : IAdapterSelection
{
	public UserControl? View => view;

	public string Id => nameof(Lagrange);

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
		=> SR.TextLagrangeAdapter;
}
