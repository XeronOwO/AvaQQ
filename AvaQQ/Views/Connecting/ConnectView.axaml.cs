using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaQQ.Adapters;
using AvaQQ.SDK.Adapters;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using ConnectConfig = AvaQQ.SDK.Configuration<AvaQQ.Configurations.ConnectConfiguration>;

namespace AvaQQ.Views.Connecting;

public partial class ConnectView : UserControl
{
	public ConnectView()
	{
		InitializeComponent();

		var defaultSelection = new SelectAdapterSelection();
		adapterSelector.Items.Add(defaultSelection);
		if (Application.Current is App app)
		{
			var selections = app.ServiceProvider.GetRequiredService<IAdapterSelectionProvider>();
			foreach (var selection in selections)
			{
				adapterSelector.Items.Add(selection);
			}
		}

		adapterSelector.SelectedItem = adapterSelector.Items.Where(
			x => x is not null && x.ToString() == ConnectConfig.Instance.AdapterIndex
		).FirstOrDefault(defaultSelection);

		Loaded += ConnectView_Loaded;
	}

	private void UpdateSelection()
	{
		ConnectConfig.Instance.AdapterIndex = string.Empty;
		if (adapterSelector.SelectedItem is not null
			&& adapterSelector.SelectedItem.ToString() is { } index)
		{
			ConnectConfig.Instance.AdapterIndex = index;
		}

		if (adapterSelector.SelectedItem is IAdapterSelection selection)
		{
			gridAdapterOptions.Children.Clear();
			var control = selection.UserControl;
			if (control is not null)
			{
				gridAdapterOptions.Children.Add(control);
			}
		}
	}

	private void ConnectView_Loaded(object? sender, RoutedEventArgs e)
	{
		UpdateSelection();
	}

	private void OnSelectAdapter(object? sender, SelectionChangedEventArgs e)
	{
		UpdateSelection();
	}
}
