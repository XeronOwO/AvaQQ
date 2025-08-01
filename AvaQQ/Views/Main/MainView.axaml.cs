using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using AvaQQ.Events;
using AvaQQ.Exceptions;
using AvaQQ.SDK;
using AvaQQ.SDK.Entities;
using AvaQQ.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ.Views.Main;

public partial class MainView : UserControl, IDisposable
{
	private readonly ICacheContext _cacheContext;

	private readonly IAdapterProvider _adapterProvider;

	private readonly AvaQQEvents _events;

	public MainViewModel ViewModel
	{
		get => (MainViewModel)(DataContext ?? throw new NotInitializedException(nameof(ViewModel)));
		set => DataContext = value;
	}

	public MainView(
		ICacheContext cacheContext,
		IAdapterProvider adapterProvider,
		AvaQQEvents events,
		MainViewModel viewModel
		)
	{
		_cacheContext = cacheContext;
		_adapterProvider = adapterProvider;
		_events = events;

		ViewModel = viewModel;
		InitializeComponent();

		DesignerHelper.ShowGridBackground(this);

		_events.OnAvatarChanged.Subscribe(OnAvatarChanged);
		_events.OnUserInfoFetched.Subscribe(OnUserInfoFetched);
	}

	public MainView() : this(
		DesignerHelper.Services.GetRequiredService<ICacheContext>(),
		DesignerHelper.Services.GetRequiredService<IAdapterProvider>(),
		DesignerHelper.Services.GetRequiredService<AvaQQEvents>(),
		DesignerHelper.Services.GetRequiredService<MainViewModel>()
		)
	{
	}

	private AvatarId _avatarId = new();

	private ulong _uin;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (_adapterProvider.Adapter is not { } adapter)
		{
			return;
		}
		_avatarId = AvatarId.User(adapter.Uin);
		SetAvatar(_cacheContext.GetAvatar(_avatarId));
		_uin = adapter.Uin;
		SetName(_cacheContext.GetUser(_uin));
	}

	private void OnAvatarChanged(object? sender, KeyedEventBusArgs<AvatarId, AvatarChangedInfo> e)
	{
		if (_adapterProvider.Adapter is null ||
			e.Key != _avatarId)
		{
			return;
		}
		SetAvatar(e.Result.New);
	}

	private void SetAvatar(Bitmap? avatar)
	{
		if (!Dispatcher.UIThread.CheckAccess())
		{
			Dispatcher.UIThread.Invoke(() => SetAvatar(avatar));
			return;
		}
		ViewModel.Avatar = avatar;
	}

	private void OnUserInfoFetched(object? sender, KeyedEventBusArgs<ulong, IUserInfo?> e)
	{
		if (_adapterProvider.Adapter is null ||
			e.Key != _uin)
		{
			return;
		}
		SetName(e.Result);
	}

	private void SetName(IUserInfo? info)
	{
		if (!Dispatcher.UIThread.CheckAccess())
		{
			Dispatcher.UIThread.Invoke(() => SetName(info));
			return;
		}
		if (info is null)
		{
			return;
		}
		ViewModel.Nickname = info.Nickname;
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_events.OnAvatarChanged.Unsubscribe(OnAvatarChanged);
			_events.OnUserInfoFetched.Unsubscribe(OnUserInfoFetched);

			if (disposing)
			{
			}

			disposedValue = true;
		}
	}

	~MainView()
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
