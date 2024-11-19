using AvaQQ.Plugins;
using AvaQQ.SDK;
using System;
using System.Threading;

namespace AvaQQ;

internal class LifetimeController(IServiceProvider serviceProvider) : ILifetimeController
{
	public CancellationTokenSource CancellationTokenSource { get; } = new();

	public void Stop()
	{
		PluginManager.UnloadPlugins(serviceProvider);

		CancellationTokenSource.Cancel();
	}
}
