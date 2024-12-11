using AvaQQ.SDK.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Onebot11ForwardWebSocketAdapter;

public class Plugin : AvaQQ.SDK.Plugin
{
	public override void OnPreload(IHostBuilder hostBuilder)
	{
		Debug.WriteLine(nameof(OnPreload));

		hostBuilder.ConfigureServices(services =>
		{
			services.AddScoped<AdapterSelection>()
				.AddScoped<AdapterSelectionView>();
		});
	}

	private ILogger<Plugin> _logger = null!;

	public override void OnLoad(IServiceProvider services)
	{
		_logger = services.GetRequiredService<ILogger<Plugin>>();
		_logger.LogInformation(nameof(OnLoad));

		services.GetRequiredService<IAdapterSelectionProvider>()
			.Register<AdapterSelection>();
	}

	public override void OnPostLoad(IServiceProvider services)
	{
		_logger.LogInformation(nameof(OnPostLoad));
	}

	public override void OnUnload()
	{
		_logger.LogInformation(nameof(OnUnload));
	}
}
