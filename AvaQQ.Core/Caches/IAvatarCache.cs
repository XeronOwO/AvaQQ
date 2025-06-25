using Avalonia.Media.Imaging;
using AvaQQ.Core.Events;

namespace AvaQQ.Core.Caches;

/// <summary>
/// 头像缓存
/// </summary>
public interface IAvatarCache : IDisposable
{
	/// <summary>
	/// 获取群头像
	/// </summary>
	/// <param name="uin">群号</param>
	/// <param name="size">
	/// 头像尺寸<br/>
	/// 仅限官方支持的尺寸，例如 0、40、100、160、640，其中 0 是原图<br/>
	/// 240可以获取默认头像，是一个企鹅（可能是特性？）
	/// </param>
	/// <param name="forceUpdate">强制更新，请订阅 <see cref="EventStation.OnGroupAvatarChanged"/> 接收更新事件</param>
	/// <returns>此方法返回的是缓存的头像，若头像不存在或者过期，会触发更新，请订阅 <see cref="EventStation.OnGroupAvatarChanged"/> 接收更新事件</returns>
	Bitmap? GetGroupAvatar(ulong uin, int size = 0, bool forceUpdate = false);

	/// <summary>
	/// 获取用户头像
	/// </summary>
	/// <param name="uin">QQ 号</param>
	/// <param name="size">
	/// 头像尺寸<br/>
	/// 仅限官方支持的尺寸，例如 0、40、100、160、640，其中 0 是原图<br/>
	/// 240可以获取默认头像，是一个企鹅（可能是特性？）
	/// </param>
	/// <param name="forceUpdate">强制更新，请订阅 <see cref="EventStation.OnUserAvatarChanged"/> 接收更新事件</param>
	/// <returns>此方法返回的是缓存的头像，若头像不存在或者过期，会触发更新，请订阅 <see cref="EventStation.OnUserAvatarChanged"/> 接收更新事件</returns>
	Bitmap? GetUserAvatar(ulong uin, int size = 0, bool forceUpdate = false);

	/// <summary>
	/// 清空缓存
	/// </summary>
	void Clear();
}
