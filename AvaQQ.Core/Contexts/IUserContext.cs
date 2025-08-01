using Avalonia.Media.Imaging;
using AvaQQ.Core.Events;

namespace AvaQQ.Core.Contexts;

/// <summary>
/// 用户上下文接口
/// </summary>
public interface IUserContext : IDisposable
{
	/// <summary>
	/// 获取缓存的头像<br/>
	/// 如果有更新，请订阅 <see cref="EventStation.OnAvatarChanged"/>
	/// </summary>
	/// <param name="uin">QQ 号</param>
	/// <param name="size">
	/// 头像尺寸<br/>
	/// 仅限官方支持的尺寸，例如 0、40、100、160、640，其中 0 是原图<br/>
	/// 240可以获取默认头像，是一个企鹅（可能是特性？）
	/// </param>
	/// <param name="forceUpdate">强制更新</param>
	Bitmap? GetAvatar(ulong uin, uint size = 0, bool forceUpdate = false);
}
