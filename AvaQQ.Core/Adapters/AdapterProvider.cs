using AvaQQ.Core.Caches;
using AvaQQ.Core.Databases;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ.Core.Adapters;

internal class AdapterProvider : IAdapterProvider
{
	private readonly Lazy<GroupMessageDatabase> _lazyGroupMessageDatabase;

	public GroupMessageDatabase GroupMessageDatabase => _lazyGroupMessageDatabase.Value;

	private readonly Lazy<IGroupCache> _lazyGroupCache;

	public IGroupCache GroupCache => _lazyGroupCache.Value;

	public AdapterProvider(IServiceProvider serviceProvider)
	{
		_lazyGroupMessageDatabase = new(serviceProvider.GetRequiredService<GroupMessageDatabase>);
		_lazyGroupCache = new(serviceProvider.GetRequiredService<IGroupCache>);

		OnAdapterChanged += AdapterProvider_OnAdapterChanged;
	}

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
			OnAdapterChanged?.Invoke(this, new AdapterChangedEventArgs(old, @new));
		}
	}

	public event EventHandler<AdapterChangedEventArgs>? OnAdapterChanged;

	private void AdapterProvider_OnAdapterChanged(object? sender, AdapterChangedEventArgs e)
	{
		if (e.Old is not null)
		{
			e.Old.OnGroupMessage -= GroupMessageDatabase.Adapter_OnGroupMessage;
			e.Old.OnGroupMessage -= GroupCache.Adapter_OnGroupMessage;
		}

		if (e.New is not null)
		{
			e.New.OnGroupMessage += GroupMessageDatabase.Adapter_OnGroupMessage;
			e.New.OnGroupMessage += GroupCache.Adapter_OnGroupMessage;
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
