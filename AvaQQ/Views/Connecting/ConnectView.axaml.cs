using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.SDK;
using AvaQQ.SDK.Adapters;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using ConnectConfig = AvaQQ.SDK.Configuration<AvaQQ.Configurations.ConnectConfiguration>;

namespace AvaQQ.Views.Connecting;

public partial class ConnectView : UserControl
{
	private readonly IServiceScope _serviceScope;

	public ConnectView()
	{
		InitializeComponent();

		var app = AppBase.Current;
		_serviceScope = app.ServiceProvider.CreateScope();
		var selectionProvider = _serviceScope.ServiceProvider.GetRequiredService<IAdapterSelectionProvider>();
		foreach (var selection in selectionProvider.CreateSelections(_serviceScope.ServiceProvider))
		{
			adapterSelector.Items.Add(selection);
		}

		adapterSelector.SelectedItem = adapterSelector.Items.Where(
			x => x is not null && x.ToString() == ConnectConfig.Instance.AdapterIndex
		).FirstOrDefault();

		Loaded += ConnectView_Loaded;
		Unloaded += ConnectView_Unloaded;
	}

	private void UpdateSelection()
	{
		ConnectConfig.Instance.AdapterIndex = string.Empty;
		if (adapterSelector.SelectedItem is not null
			&& adapterSelector.SelectedItem.ToString() is { } index)
		{
			ConnectConfig.Instance.AdapterIndex = index;
		}

		if (adapterSelector.SelectedItem is not IAdapterSelection selection)
		{
			return;
		}

		gridAdapterOptions.Children.Clear();
		var control = selection.UserControl;
		if (control is not null)
		{
			gridAdapterOptions.Children.Add(control);
		}
	}

	private void ConnectView_Loaded(object? sender, RoutedEventArgs e)
	{
		UpdateSelection();
	}

	private void ConnectView_Unloaded(object? sender, RoutedEventArgs e)
	{
		_serviceScope.Dispose();
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
