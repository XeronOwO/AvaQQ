using AvaQQ.SDK.Adapters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace AvaQQ.Adapters;

internal class AdapterSelectionProvider : IAdapterSelectionProvider
{
	private readonly List<Type> _adapters = [];

	private readonly ILogger<AdapterSelectionProvider> _logger;

	public AdapterSelectionProvider(IServiceProvider serviceProvider)
	{
		_logger = serviceProvider.GetRequiredService<ILogger<AdapterSelectionProvider>>();

		Register<SelectAdapterSelection>();
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
			if (scopedServiceProvider.GetService(type) is IAdapterSelection selection)
			{
				selections.Add(selection);
			}
			else
			{
				_logger.LogWarning("Failed to create adapter selection from type {Type}.", type);
			}
		}
		return selections;
	}
}
