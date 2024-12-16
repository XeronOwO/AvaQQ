using AvaQQ.Core.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AvaQQ.Core.Adapters;

internal class AdapterSelectionProvider : IAdapterSelectionProvider
{
	private readonly List<Type> _adapters = [];

	private readonly ILogger<AdapterSelectionProvider> _logger;

	public AdapterSelectionProvider(IServiceProvider serviceProvider)
	{
		CirculationInjectionDetector<AdapterSelectionProvider>.Enter();

		_logger = serviceProvider.GetRequiredService<ILogger<AdapterSelectionProvider>>();

		Register<SelectAdapterSelection>();

		CirculationInjectionDetector<AdapterSelectionProvider>.Leave();
	}

	public void Register<T>() where T : IAdapterSelection
		=> Register(typeof(T));

	private static readonly Type _adapterSelectionType = typeof(IAdapterSelection);

	public void Register(Type type)
	{
		try
		{
			if (!type.IsAssignableTo(_adapterSelectionType))
			{
				throw new ArgumentException($"Type {type} is not assignable to {_adapterSelectionType}.");
			}

			_adapters.Add(type);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to register adapter selection.");
		}
	}

	public List<IAdapterSelection> CreateSelections(IServiceProvider scopedServiceProvider)
	{
		var selections = new List<IAdapterSelection>();
		foreach (var type in _adapters)
		{
			try
			{
				selections.Add((IAdapterSelection)scopedServiceProvider.GetRequiredService(type));
			}
			catch (Exception e)
			{
				_logger.LogWarning(e, "Failed to create adapter selection from type {Type}.", type);
			}
		}
		return selections;
	}
}
