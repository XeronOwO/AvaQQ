using AvaQQ.Resources;
using AvaQQ.SDK;

namespace AvaQQ.Adapters;

internal class AdapterProvider : IAdapterProvider
{
	public IAdapter? Adapter { get; set; }

	public IAdapter EnsuredAdapter
		=> Adapter ?? throw new InvalidOperationException(SR.ExceptionAdapterIsNull);

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
