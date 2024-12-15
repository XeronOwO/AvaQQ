using AvaQQ.Core.Adapters;
using AvaQQ.SDK;
using AvaQQ.SDK.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Onebot11ForwardWebSocketAdapter;

public class Ob11FWSPlugin : Plugin
{
	public override void OnPreLoad(IHostBuilder hostBuilder)
	{
		hostBuilder.ConfigureServices(services =>
		{
			services.AddScoped<AdapterSelection>()
				.AddScoped<AdapterSelectionView>();
		});

		FileLoggingExecutor.Information<Ob11FWSPlugin>($"{nameof(Ob11FWSPlugin)} preloaded.");
	}

	private ILogger<Ob11FWSPlugin> _logger = null!;

	public override void OnLoad(IServiceProvider services)
	{
		_logger = services.GetRequiredService<ILogger<Ob11FWSPlugin>>();
		_logger.LogInformation(nameof(OnLoad));

		services.GetRequiredService<IAdapterSelectionProvider>()
			.Register<AdapterSelection>();

		_logger.LogInformation($"{nameof(Ob11FWSPlugin)} loaded.");
	}

	public override void OnPostLoad(IServiceProvider services)
	{
		_logger.LogInformation($"{nameof(Ob11FWSPlugin)} post loaded.");
	}

	public override void OnUnload()
	{
		_logger.LogInformation($"{nameof(Ob11FWSPlugin)} unloaded.");
	}
}
