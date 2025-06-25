using Avalonia.Controls;
using Avalonia.Threading;
using AvaQQ.Core.Adapters;
using AvaQQ.Core.Caches;
using AvaQQ.Core.Events;
using AvaQQ.Core.Utils;
using AvaQQ.Core.ViewModels.MainPanels;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.MainPanelConfiguration>;

namespace AvaQQ.Core.Views.MainPanels;

/// <summary>
/// 主面板窗口
/// </summary>
public partial class MainPanelWindow : Window
{
	private readonly IAdapterProvider _adapterProvider;

	private readonly EventStation _events;

	/// <summary>
	/// 创建主面板窗口
	/// </summary>
	public MainPanelWindow(
		IAdapterProvider adapterProvider,
		IAvatarCache avatarCache,
		IUserCache userCache,
		MainPanelView view,
		EventStation events
		)
	{
		CirculationInjectionDetector<MainPanelWindow>.Enter();

		_adapterProvider = adapterProvider;
		_events = events;
		var model = new MainPanelViewModel();
		DataContext = model;
		InitializeComponent();
		gridMainPanelView.Children.Add(view);

		Closed += (_, _) =>
		{
			Config.Save();
		};

		CirculationInjectionDetector<MainPanelWindow>.Leave();

		_events.OnUserAvatarChanged.Subscribe(OnUserAvatarChanged);
		_events.OnUserFetched.Subscribe(OnUserFetched);
		Closed += (_, _) =>
		{
			_events.OnUserAvatarChanged.Unsubscribe(OnUserAvatarChanged);
			_events.OnUserFetched.Unsubscribe(OnUserFetched);
		};

		if (adapterProvider.Adapter is { } adapter)
		{
			model.HeaderUin = adapter.Uin;
			model.HeaderAvatar = avatarCache.GetUserAvatar(adapter.Uin, 40);
			model.HeaderName = userCache.GetUser(adapter.Uin)?.Nickname ?? string.Empty;
		}
	}

	private void OnUserFetched(object? sender, BusEventArgs<UinId, AdaptedUserInfo?> e)
	{
		if (_adapterProvider.Adapter is not { } adapter ||
			e.Id.Uin != adapter.Uin)
		{
			return;
		}

		Dispatcher.UIThread.Invoke(() =>
		{
			if (DataContext is not MainPanelViewModel model)
			{
				return;
			}

			model.HeaderName = e.Result?.Nickname ?? string.Empty;
		});
	}

	private void OnUserAvatarChanged(object? sender, BusEventArgs<AvatarId, AvatarChangedInfo> e)
	{
		if (_adapterProvider.Adapter is not { } adapter ||
			e.Id.Uin != adapter.Uin)
		{
			return;
		}

		Dispatcher.UIThread.Invoke(() =>
		{
			if (DataContext is not MainPanelViewModel model)
			{
				return;
			}

			model.HeaderAvatar = e.Result.New;
		});
	}

	/// <summary>
	/// 创建主面板窗口
	/// </summary>
	public MainPanelWindow() : this(
		DesignerServiceProviderHelper.Root.GetRequiredService<IAdapterProvider>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<IAvatarCache>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<IUserCache>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<MainPanelView>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<EventStation>()
		)
	{
	}
}
