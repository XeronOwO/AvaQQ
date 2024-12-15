using Avalonia.Controls;
using AvaQQ.Core.Adapters;
using AvaQQ.Core.ViewModels;
using AvaQQ.SDK;
using Microsoft.Extensions.DependencyInjection;
using Config = AvaQQ.SDK.Configuration<AvaQQ.Core.Configurations.ConnectConfiguration>;

namespace AvaQQ.Core.Views.Connecting;

/// <summary>
/// ���Ӵ���
/// </summary>
public partial class ConnectWindow : Window
{
	private readonly IAdapterProvider _adapterProvider;

	/// <summary>
	/// �������Ӵ���
	/// </summary>
	public ConnectWindow(
		ConnectView connectView,
		IAdapterProvider adapterProvider
		)
	{
		_adapterProvider = adapterProvider;

		DataContext = new ConnectViewModel();
		InitializeComponent();
		gridConnectView.Children.Add(connectView);

		Closed += ConnectWindow_Closed;
	}

	/// <summary>
	/// �������Ӵ���
	/// </summary>
	public ConnectWindow() : this(
		DesignerServiceProviderHelper.Root.GetRequiredService<ConnectView>(),
		DesignerServiceProviderHelper.Root.GetRequiredService<IAdapterProvider>()
		)
	{
	}

	/// <summary>
	/// ��ʼ����<br/>
	/// ����ý����ϵİ�ť�ȿɲ����ؼ�
	/// </summary>
	public void BeginConnect()
	{
		if (DataContext is not ConnectViewModel model)
		{
			return;
		}

		model.IsConnecting = true;
	}

	/// <summary>
	/// ��������
	/// </summary>
	/// <param name="adapter">
	/// ������<br/>
	/// �������ʧ�ܣ������� null����ָ��ؼ�״̬������رմ���<br/>
	/// ������ӳɹ���������������ʵ����������������
	/// </param>
	public void EndConnect(IAdapter? adapter)
	{
		if (DataContext is not ConnectViewModel model)
		{
			return;
		}

		model.IsConnecting = false;

		if (adapter is null)
		{
			return;
		}

		_adapterProvider.Adapter = adapter;

		Close();
	}

	private void ConnectWindow_Closed(object? sender, EventArgs e)
	{
		Config.Save();
	}
}
