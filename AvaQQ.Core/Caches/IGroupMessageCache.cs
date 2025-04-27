namespace AvaQQ.Core.Caches;

/// <summary>
/// 群消息缓存接口
/// </summary>
public interface IGroupMessageCache : IDisposable
{
	/// <summary>
	/// 最新信息的时间
	/// </summary>
	/// <param name="uin">群号</param>
	string GetLatestMessageTime(ulong uin);

	/// <summary>
	/// 最新信息的预览
	/// </summary>
	/// <param name="uin">群号</param>
	string GetLatestMessagePreview(ulong uin);
}
