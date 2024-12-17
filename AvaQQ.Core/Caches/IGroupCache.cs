using AvaQQ.Core.Adapters;
using AvaQQ.Core.Databases;

namespace AvaQQ.Core.Caches;

/// <summary>
/// 群缓存
/// </summary>
public interface IGroupCache
{
	/// <summary>
	/// 手动更新群聊列表缓存
	/// </summary>
	Task UpdateGroupListAsync();

	/// <summary>
	/// 获取所有群聊简略信息
	/// </summary>
	/// <param name="noCache">是否启用缓存</param>
	Task<IEnumerable<GroupInfo>> GetAllGroupInfosAsync(bool noCache = false);

	/// <summary>
	/// 获取群聊简略信息
	/// </summary>
	/// <param name="uin">群号</param>
	/// <param name="noCache">是否启用缓存</param>
	Task<GroupInfo?> GetGroupInfoAsync(ulong uin, bool noCache = false);

	/// <summary>
	/// 获取群聊名称
	/// </summary>
	/// <param name="uin">群号</param>
	Task<string> GetGroupNameAsync(ulong uin);

	/// <summary>
	/// 获取群聊最新消息
	/// </summary>
	/// <param name="group">群号</param>
	Task<GroupMessageEntry?> GetLatestMessageEntryAsync(ulong group);

	/// <summary>
	/// 获取群聊最新消息时间
	/// </summary>
	/// <param name="group">群号</param>
	Task<string> GetLatestMessageTimeAsync(ulong group);

	/// <summary>
	/// 获取群聊最新消息预览
	/// </summary>
	/// <param name="group">群号</param>
	Task<string> GetLatestMessagePreviewAsync(ulong group);

	/// <summary>
	/// 当收到群消息时触发
	/// </summary>
	void Adapter_OnGroupMessage(object? sender, GroupMessageEventArgs e);
}
