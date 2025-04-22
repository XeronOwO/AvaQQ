using AvaQQ.Core.Adapters;
using AvaQQ.SDK;
using AvaQQ.SDK.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AvaQQ.Adapters.Lagrange;

public class LagrangePlugin : Plugin
{
	public override void OnPreLoad(IHostBuilder hostBuilder)
	{
		hostBuilder.ConfigureServices(services =>
		{
			services.AddScoped<AdapterSelection>()
				.AddScoped<AdapterSelectionView>();
		});

		FileLoggingExecutor.Information<LagrangePlugin>($"{nameof(LagrangePlugin)} preloaded.");
	}

	private ILogger<LagrangePlugin> _logger = null!;

	public override void OnLoad(IServiceProvider services)
	{
		_logger = services.GetRequiredService<ILogger<LagrangePlugin>>();

		services.GetRequiredService<IAdapterSelectionProvider>()
			.Register<AdapterSelection>();

		_logger.LogInformation($"{nameof(LagrangePlugin)} loaded.");
	}

	public override void OnPostLoad(IServiceProvider services)
	{
		_logger.LogInformation($"{nameof(LagrangePlugin)} post loaded.");
	}

	public override void OnUnload()
	{
		_logger.LogInformation($"{nameof(LagrangePlugin)} unloaded.");
	}
}
