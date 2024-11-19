using AvaQQ.SDK;
using System.Threading;

namespace AvaQQ;

internal class LifetimeController : ILifetimeController
{
	public CancellationTokenSource CancellationTokenSource { get; } = new();
}
