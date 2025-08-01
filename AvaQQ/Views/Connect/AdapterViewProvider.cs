using AvaQQ.SDK;
using Microsoft.Extensions.Logging;

namespace AvaQQ.Views.Connect;

internal class AdapterViewProvider(ILogger<AdapterViewProvider> logger) : IAdapterViewProvider
{
	public Dictionary<string, IAdapterView> Views { get; } = [];

	public void Register(IAdapterView view)
	{
		logger.LogInformation("Registering adapter view: {Id}", view.Id);

		Views.Add(view.Id, view);
	}

	public void Release()
	{
		foreach (var (_, view) in Views)
		{
			view.OnRelease();
		}
	}
}
