using Avalonia.Controls;
using Avalonia.Threading;
using AvaQQ.Events;
using AvaQQ.Exceptions;
using AvaQQ.SDK;
using AvaQQ.SDK.Entities;
using AvaQQ.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ.Views.Main;

public partial class MainWindow : Window, IDisposable
{
	private readonly IAdapterProvider _adapterProvider;

	private readonly ICacheContext _cacheContext;

	private readonly AvaQQEvents _events;

	public MainWindowViewModel ViewModel
	{
		get => (MainWindowViewModel)(DataContext ?? throw new NotInitializedException(nameof(ViewModel)));
		set => DataContext = value;
	}

	public MainWindow(
		IAdapterProvider adapterProvider,
		ICacheContext cacheContext,
		AvaQQEvents events,
		MainWindowViewModel viewModel,
		MainView view
		)
	{
		_adapterProvider = adapterProvider;
		_cacheContext = cacheContext;
		_events = events;

		ViewModel = viewModel;
		InitializeComponent();
		_panel.Children.Add(view);

		_events.OnUserInfoFetched.Subscribe(OnUserInfoFetched);
	}

	public MainWindow() : this(
		DesignerHelper.Services.GetRequiredService<IAdapterProvider>(),
		DesignerHelper.Services.GetRequiredService<ICacheContext>(),
		DesignerHelper.Services.GetRequiredService<AvaQQEvents>(),
		DesignerHelper.Services.GetRequiredService<MainWindowViewModel>(),
		DesignerHelper.Services.GetRequiredService<MainView>()
		)
	{
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (_adapterProvider.Adapter is { } adapter)
		{
			SetTitle(_cacheContext.GetUser(adapter.Uin));
		}
	}

	private void OnUserInfoFetched(object? sender, KeyedEventBusArgs<ulong, IUserInfo?> e)
	{
		if (e.Result is not { } result)
		{
			return;
		}
		SetTitle(result);
	}

	private void SetTitle(IUserInfo? info)
	{
		if (!Dispatcher.UIThread.CheckAccess())
		{
			Dispatcher.UIThread.Post(() => SetTitle(info));
			return;
		}
		if (info is null)
		{
			return;
		}
		ViewModel.Title = info.Nickname;
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_events.OnUserInfoFetched.Unsubscribe(OnUserInfoFetched);

			if (disposing)
			{
			}

			disposedValue = true;
		}
	}

	~MainWindow()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion
}
