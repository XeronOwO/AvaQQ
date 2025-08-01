using AvaQQ.Events;
using AvaQQ.Exceptions;
using AvaQQ.SDK;
using AvaQQ.ViewModels.Connect;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ.Views.Connect;

public partial class ConnectWindow : ConnectWindowBase
{
	private readonly ConnectView _view;

	private readonly IAppLifetime _lifetime;

	private readonly IDatabase _database;

	private readonly IAdapterProvider _adapterProvider;

	private readonly IMainWindowProvider _mainWindowProvider;

	private readonly AvaQQEvents _events;

	public ConnectWindowViewModel ViewModel
	{
		get => (ConnectWindowViewModel)(DataContext ?? throw new NotInitializedException(nameof(ViewModel)));
		set => DataContext = value;
	}

	public ConnectWindow(
		ConnectView view,
		IAppLifetime lifetime,
		IDatabase database,
		IAdapterProvider adapterProvider,
		IMainWindowProvider mainWindowProvider,
		AvaQQEvents events,
		ConnectWindowViewModel viewModel
		)
	{
		_view = view;
		_lifetime = lifetime;
		_database = database;
		_adapterProvider = adapterProvider;
		_mainWindowProvider = mainWindowProvider;
		_events = events;

		ViewModel = viewModel;
		InitializeComponent();
		_panel.Children.Add(view);
	}

	public ConnectWindow() : this(
		DesignerHelper.Services.GetRequiredService<ConnectView>(),
		DesignerHelper.Services.GetRequiredService<IAppLifetime>(),
		DesignerHelper.Services.GetRequiredService<IDatabase>(),
		DesignerHelper.Services.GetRequiredService<IAdapterProvider>(),
		DesignerHelper.Services.GetRequiredService<IMainWindowProvider>(),
		DesignerHelper.Services.GetRequiredService<AvaQQEvents>(),
		DesignerHelper.Services.GetRequiredService<ConnectWindowViewModel>()
		)
	{
	}

	public override void BeginConnect()
		=> _view.BeginConnect();

	public override void EndConnect(IAdapter? adapter)
	{
		if (_view.EndConnect(adapter))
		{
			Close();
		}
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);

		if (_adapterProvider.Adapter is not { } adapter)
		{
			_lifetime.Shutdown();
			return;
		}

		_database.Initialize(adapter.Uin);
		_events.OnLoggedIn.Invoke(adapter);
		_mainWindowProvider.OpenOrActivateMainWindow();
	}
}
