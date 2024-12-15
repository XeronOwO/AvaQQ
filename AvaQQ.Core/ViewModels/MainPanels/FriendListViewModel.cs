using AvaQQ.Core.Resources;
using AvaQQ.SDK;

namespace AvaQQ.Core.ViewModels.MainPanels;

/// <summary>
/// 好友列表视图模型
/// </summary>
public class FriendListViewModel : ViewModelBase
{
	/// <summary>
	/// 过滤水印
	/// </summary>
	public string FilterWatermark => SR.TextInputFilterKeywords;
}
