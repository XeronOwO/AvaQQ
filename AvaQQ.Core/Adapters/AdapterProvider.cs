using AvaQQ.Core.Databases;

namespace AvaQQ.Core.Adapters;

internal class AdapterProvider(Database database) : IAdapterProvider
{
	private IAdapter? _adapter;

	public IAdapter? Adapter
	{
		get => _adapter;
		set
		{
			var old = _adapter;
			var @new = value;
			if (old == @new)
			{
				return;
			}

			_adapter = @new;
			Unregister(old);
			Register(@new);
		}
	}

	private void Register(IAdapter? adapter)
	{
		if (adapter == null)
		{
			return;
		}

		database.Initialize(adapter.Uin);
	}

	private void Unregister(IAdapter? adapter)
	{
		if (adapter == null)
		{
			return;
		}
	}

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
