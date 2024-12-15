using Avalonia.Controls;
using AvaQQ.Core.Resources;

namespace AvaQQ.Core.Adapters;

internal class SelectAdapterSelection : IAdapterSelection
{
	public UserControl? View => null;

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
	{
		return SR.TextSelectAdapter;
	}
}
