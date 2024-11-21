using AvaQQ.SDK.Adapters;
using System.Collections.Generic;

namespace AvaQQ.Adapters;

internal class AdapterSelectionProvider : List<IAdapterSelection>, IAdapterSelectionProvider
{
	public AdapterSelectionProvider()
	{
		Add(new SelectAdapterSelection());
	}
}
