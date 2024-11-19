using Avalonia.Controls;
using Avalonia.Interactivity;
using ConnectConfig = AvaQQ.SDK.Configuration<AvaQQ.Configurations.ConnectConfiguration>;

namespace AvaQQ.Views.Connecting;

public partial class ConnectView : UserControl
{
	public ConnectView()
	{
		InitializeComponent();

		adapterSelector.Items.Add(new NoneAdapterSelection());
		adapterSelector.Items.Add(new Onebot11ForwardWebSocketAdapterSelection());
		adapterSelector.SelectedIndex = ConnectConfig.Instance.AdapterIndex;

		Loaded += ConnectView_Loaded;
	}

	private void UpdateSelection()
	{
		ConnectConfig.Instance.AdapterIndex = adapterSelector.SelectedIndex;

		if (adapterSelector.SelectedItem is AdapterSelection selection)
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
