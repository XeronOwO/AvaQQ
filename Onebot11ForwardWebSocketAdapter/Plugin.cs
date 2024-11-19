using Microsoft.Extensions.Hosting;

namespace Onebot11ForwardWebSocketAdapter;

public class Plugin : AvaQQ.SDK.Plugin
{
	public override void OnPreload(IHostBuilder hostBuilder)
	{
		throw new Exception($"{nameof(OnPreload)} Exception");
	}

	public override void OnLoad(IServiceProvider serviceProvider)
	{
		throw new Exception($"{nameof(OnLoad)} Exception");
	}

	public override void OnUnload()
	{
		throw new Exception($"{nameof(OnUnload)} Exception");
	}
}
