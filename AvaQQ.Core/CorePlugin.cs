using AvaQQ.SDK;
using AvaQQ.SDK.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AvaQQ.Core;

internal class CorePlugin : Plugin
{
	public override void OnPreLoad(IHostBuilder hostBuilder)
	{
		hostBuilder.ConfigureAvaQQCore();

		FileLoggingExecutor.Information<CorePlugin>($"{nameof(CorePlugin)} preloaded.");
	}

	private ILogger<CorePlugin> _logger = null!;

	public override void OnLoad(IServiceProvider services)
	{
		_logger = services.GetRequiredService<ILogger<CorePlugin>>();

		_logger.LogInformation($"{nameof(CorePlugin)} loaded.");
	}

	public override void OnPostLoad(IServiceProvider services)
	{
		_logger.LogInformation($"{nameof(CorePlugin)} post loaded.");
	}

	public override void OnUnload()
	{
		_logger.LogInformation($"{nameof(CorePlugin)} unloaded.");
	}
}
