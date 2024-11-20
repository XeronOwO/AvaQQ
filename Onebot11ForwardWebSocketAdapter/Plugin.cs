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
			services.AddSingleton<AdapterSelection>();
		});
	}

	private ILogger<Plugin> _logger = null!;

	public override void OnLoad(IServiceProvider services)
	{
		_logger = services.GetRequiredService<ILogger<Plugin>>();
		_logger.LogInformation(nameof(OnLoad));

		services.GetRequiredService<IAdapterSelectionProvider>().Add(
			services.GetRequiredService<AdapterSelection>()
		);
	}

	public override void OnUnload()
	{
		_logger.LogInformation(nameof(OnUnload));
	}
}
