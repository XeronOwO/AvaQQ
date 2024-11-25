using Avalonia.Controls;
using AvaQQ.Resources;
using AvaQQ.SDK.Adapters;

namespace AvaQQ.Adapters;

internal class SelectAdapterSelection : IAdapterSelection
{
	public UserControl? UserControl => null;

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
