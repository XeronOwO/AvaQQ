using Avalonia.Controls;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Configurations.ConnectConfiguration>;

namespace AvaQQ.Views.Connecting;

public partial class ConnectView : UserControl
{
	private readonly IServiceProvider _serviceProvider;

	private readonly IAdapterSelectionProvider _adapterSelectionProvider;

	public ConnectView(
		IServiceProvider serviceProvider,
		IAdapterSelectionProvider adapterSelectionProvider
		)
	{
		_serviceProvider = serviceProvider;
		_adapterSelectionProvider = adapterSelectionProvider;

		InitializeComponent();

		var selections = _adapterSelectionProvider.CreateSelections(_serviceProvider);
		foreach (var selection in selections)
		{
			adapterSelector.Items.Add(selection);
		}
		adapterSelector.SelectedItem = adapterSelector.Items.Where(
			x => x is not null && x.ToString() == Config.Instance.AdapterIndex
		).FirstOrDefault();
		UpdateSelection();
	}

	public ConnectView() : this(
		DesignerServiceProviderHelper.Root,
		DesignerServiceProviderHelper.Root.GetRequiredService<IAdapterSelectionProvider>()
		)
	{
	}

	private void UpdateSelection()
	{
		Config.Instance.AdapterIndex = string.Empty;
		if (adapterSelector.SelectedItem is not null
			&& adapterSelector.SelectedItem.ToString() is { } index)
		{
			Config.Instance.AdapterIndex = index;
		}

		if (adapterSelector.SelectedItem is not IAdapterSelection selection)
		{
			return;
		}

		gridAdapterOptions.Children.Clear();
		var control = selection.View;
		if (control is not null)
		{
			gridAdapterOptions.Children.Add(control);
		}
	}

	private void OnSelectAdapter(object? sender, SelectionChangedEventArgs e)
	{
		if (e.RemovedItems is not null)
		{
			foreach (var item in e.RemovedItems)
			{
				if (item is IAdapterSelection selection)
				{
					selection.OnDeselected();
				}
			}
		}

		UpdateSelection();

		if (e.AddedItems is not null)
		{
			foreach (var item in e.AddedItems)
			{
				if (item is IAdapterSelection selection)
				{
					selection.OnSelected();
				}
			}
		}
	}
}
