namespace AvaQQ.Core.Adapters;

internal class AdapterProvider : IAdapterProvider
{
	public IAdapter? Adapter { get; set; }

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				Adapter?.Dispose();
			}

			disposedValue = true;
		}
	}

	~AdapterProvider()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion
}
