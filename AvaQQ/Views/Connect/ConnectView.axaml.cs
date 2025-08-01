using Avalonia.Controls;
using Avalonia.LogicalTree;
using AvaQQ.Exceptions;
using AvaQQ.SDK;
using AvaQQ.ViewModels.Connect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Configurations.ConnectConfiguration>;

namespace AvaQQ.Views.Connect;

public partial class ConnectView : UserControl
{
	private readonly ILogger<ConnectView> _logger;

	private readonly IAdapterProvider _adapterProvider;

	private readonly IAdapterViewProvider _provider;

	public ConnectViewModel ViewModel
	{
		get => (ConnectViewModel)(DataContext ?? throw new NotInitializedException(nameof(ViewModel)));
		set => DataContext = value;
	}

	public ConnectView(
		ILogger<ConnectView> logger,
		IAdapterProvider adapterProvider,
		IAdapterViewProvider provider,
		ConnectViewModel viewModel
		)
	{
		_logger = logger;
		_adapterProvider = adapterProvider;
		_provider = provider;

		ViewModel = viewModel;
		InitializeComponent();
	}

	public ConnectView() : this(
		DesignerHelper.Services.GetRequiredService<ILogger<ConnectView>>(),
		DesignerHelper.Services.GetRequiredService<IAdapterProvider>(),
		DesignerHelper.Services.GetRequiredService<IAdapterViewProvider>(),
		DesignerHelper.Services.GetRequiredService<ConnectViewModel>()
		)
	{
	}

	private bool _isAdapterSelectorInitialized;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		foreach (var (_, adapter) in _provider.Views)
		{
			adapterSelector.Items.Add(adapter);

			if (adapter.Id == Config.Instance.SelectedAdapter)
			{
				adapterSelector.SelectedItem = adapter;
			}
		}

		_isAdapterSelectorInitialized = true;
		UpdateAdapterView();
	}

	protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromLogicalTree(e);

		gridAdapterOptions.Children.Clear();
		_provider.Release();
	}

	private void OnSelectAdapter(object? sender, SelectionChangedEventArgs e)
	{
		if (!_isAdapterSelectorInitialized)
		{
			return;
		}

		foreach (var item in e.RemovedItems)
		{
			if (item is IAdapterView view)
			{
				view.OnDeselected();
			}
		}

		UpdateAdapterView();

		foreach (var item in e.AddedItems)
		{
			if (item is IAdapterView view)
			{
				view.OnSelected();
			}
		}
	}

	private void UpdateAdapterView()
	{
		Config.Instance.SelectedAdapter = string.Empty;

		if (adapterSelector.SelectedItem is not IAdapterView view)
		{
			return;
		}

		Config.Instance.SelectedAdapter = view.Id;
		Config.Save();

		gridAdapterOptions.Children.Clear();
		gridAdapterOptions.Children.Add(view.View);
	}

	public void BeginConnect()
	{
		_logger.LogInformation("Begin connecting");

		ViewModel.IsConnecting = true;
	}

	public bool EndConnect(IAdapter? adapter)
	{
		_logger.LogInformation("End connecting");

		ViewModel.IsConnecting = false;

		if (adapter is null)
		{
			_logger.LogInformation("Connect failed. Restoring the state of controls");
			return false;
		}

		_logger.LogInformation($"Connected successfully");
		_adapterProvider.Adapter = adapter;
		return true;
	}
}
