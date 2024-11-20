using Avalonia.Controls;
using AvaQQ.Resources;
using AvaQQ.SDK.Adapters;

namespace AvaQQ.Adapters;

internal class SelectAdapterSelection : IAdapterSelection
{
	public UserControl? UserControl => null;

	public override string ToString()
	{
		return SR.TextSelectAdapter;
	}
}
