using AvaQQ.Core.Resources;
using AvaQQ.SDK;
using ReactiveUI;

namespace AvaQQ.Core.ViewModels;

/// <summary>
/// 连接视图模型
/// </summary>
public class ConnectViewModel : ViewModelBase
{
	/// <summary>
	/// 应用名称
	/// </summary>
	public string AppName => SR.AppName;

	/// <summary>
	/// 连接文本
	/// </summary>
	public string TextConnect => SR.TextConnect;

	/// <summary>
	/// 输入Url文本
	/// </summary>
	public string TextInputUrl => SR.TextInputUrl;

	/// <summary>
	/// 输入访问令牌文本
	/// </summary>
	public string TextInputAccessToken => SR.TextInputAccessToken;

	private bool _isConnecting = false;

	/// <summary>
	/// 是否正在连接
	/// </summary>
	public bool IsConnecting
	{
		get => _isConnecting;
		set
		{
			this.RaiseAndSetIfChanged(ref _isConnecting, value);
			IsNotConnecting = !value;
		}
	}

	private bool _isNotConnecting = true;

	/// <summary>
	/// 是否未连接
	/// </summary>
	public bool IsNotConnecting
	{
		get => _isNotConnecting;
		private set => this.RaiseAndSetIfChanged(ref _isNotConnecting, value);
	}
}
