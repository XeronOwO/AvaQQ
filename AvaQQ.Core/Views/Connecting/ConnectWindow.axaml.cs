using Avalonia.Controls;
using AvaQQ.Core.Adapters;
using AvaQQ.Core.Utils;
using AvaQQ.Core.ViewModels;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.ConnectConfiguration>;

namespace AvaQQ.Core.Views.Connecting;

/// <summary>
/// 连接窗口
/// </summary>
public partial class ConnectWindow : Window
{
	private readonly IAdapterProvider _adapterProvider;

	private readonly ILogger<ConnectWindow> _logger;

	/// <summary>
	/// 创建连接窗口
	/// </summary>
	public ConnectWindow(
		ConnectView connectView,
		IAdapterProvider adapterProvider,
		ILogger<ConnectWindow> logger
		)
	{
		CirculationInjectionDetector<ConnectWindow>.Enter();

		_adapterProvider = adapterProvider;
		_logger = logger;

		_logger.LogInformation("Creating ConnectWindow.");

		DataContext = new ConnectViewModel();
		InitializeComponent();
		gridConnectView.Children.Add(connectView);

		Closed += ConnectWindow_Closed;

		CirculationInjectionDetector<ConnectWindow>.Leave();
	}

	/// <summary>
	/// 创建连接窗口
	/// </summary>
	public ConnectWindow() : this(
		DesignerServiceProviderHelper.Root.GetRequiredService<ConnectView>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<IAdapterProvider>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<ILogger<ConnectWindow>>()
		)
	{
	}

	/// <summary>
	/// 开始连接<br/>
	/// 会禁用界面上的按钮等可操作控件
	/// </summary>
	public void BeginConnect()
	{
		_logger.LogInformation("Begin connecting.");

		if (DataContext is not ConnectViewModel model)
		{
			_logger.LogError("DataContext is not {Type}. Skipping...", nameof(ConnectViewModel));
			return;
		}

		model.IsConnecting = true;
	}

	/// <summary>
	/// 结束连接
	/// </summary>
	/// <param name="adapter">
	/// 适配器<br/>
	/// 如果连接失败，则设置 null，会恢复控件状态，不会关闭窗口<br/>
	/// 如果连接成功，则设置适配器实例，后续打开主窗口
	/// </param>
	public void EndConnect(IAdapter? adapter)
	{
		_logger.LogInformation("End connecting.");

		if (DataContext is not ConnectViewModel model)
		{
			_logger.LogError("DataContext is not {Type}. Skipping...", nameof(ConnectViewModel));
			return;
		}

		model.IsConnecting = false;

		if (adapter is null)
		{
			_logger.LogInformation("Connect failed. Restoring the state of controls.");
			return;
		}

		_logger.LogInformation("Connected successfully. Closing {Window}.", nameof(ConnectWindow));

		_adapterProvider.Adapter = adapter;
		Close();
	}

	private void ConnectWindow_Closed(object? sender, EventArgs e)
	{
		Config.Save();
	}
}
