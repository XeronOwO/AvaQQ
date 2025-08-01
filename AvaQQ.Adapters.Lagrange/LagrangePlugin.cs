using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AvaQQ.Adapters.Lagrange;

public class LagrangePlugin : Plugin
{
	private ILogger<LagrangePlugin> _logger = null!;

	public override void OnPreLoad()
	{
		_logger = AppBase.Current.Services.GetRequiredService<ILogger<LagrangePlugin>>();
		var viewProvider = AppBase.Current.Services.GetRequiredService<IAdapterViewProvider>();
		viewProvider.Register(new AdapterViewImpl());

		_logger.LogInformation($"{nameof(LagrangePlugin)} preloaded");
	}

	public override void OnLoad()
	{
		_logger.LogInformation($"{nameof(LagrangePlugin)} loaded");
	}

	public override void OnPostLoad()
	{
		_logger.LogInformation($"{nameof(LagrangePlugin)} post loaded");
	}

	public override void OnUnload()
	{
		_logger.LogInformation($"{nameof(LagrangePlugin)} unloaded");
	}
}
