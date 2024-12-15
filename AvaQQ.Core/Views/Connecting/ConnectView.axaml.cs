using Avalonia.Controls;
using AvaQQ.Core.Adapters;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.ConnectConfiguration>;

namespace AvaQQ.Core.Views.Connecting;

/// <summary>
/// 连接视图
/// </summary>
public partial class ConnectView : UserControl
{
	private readonly IServiceProvider _serviceProvider;

	private readonly IAdapterSelectionProvider _adapterSelectionProvider;

	/// <summary>
	/// 创建连接视图
	/// </summary>
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
			x => x is IAdapterSelection adapterSelection
				&& adapterSelection.Id == Config.Instance.SelectedAdapter
		).FirstOrDefault();
		UpdateSelection();
	}

	/// <summary>
	/// 创建连接视图
	/// </summary>
	public ConnectView() : this(
		DesignerServiceProviderHelper.Root,
		DesignerServiceProviderHelper.Root.GetRequiredService<IAdapterSelectionProvider>()
		)
	{
	}

	private void UpdateSelection()
	{
		Config.Instance.SelectedAdapter = string.Empty;

		if (adapterSelector.SelectedItem is not IAdapterSelection selection)
		{
			return;
		}

		Config.Instance.SelectedAdapter = selection.Id;

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
