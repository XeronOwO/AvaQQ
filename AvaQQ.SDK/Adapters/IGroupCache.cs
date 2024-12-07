using AvaQQ.SDK.Databases;
using AvaQQ.SDK.Messages;
using System.Threading.Tasks;

namespace AvaQQ.SDK.Adapters;

/// <summary>
/// 群缓存
/// </summary>
public interface IGroupCache
{
	/// <summary>
	/// 获取所有群聊简略信息
	/// </summary>
	/// <param name="noCache">是否启用缓存</param>
	Task<GroupInfo[]> GetAllGroupInfosAsync(bool noCache = false);

	/// <summary>
	/// 获取群聊简略信息
	/// </summary>
	/// <param name="uin">群号</param>
	/// <param name="noCache">是否启用缓存</param>
	Task<GroupInfo?> GetGroupInfoAsync(ulong uin, bool noCache = false);

	/// <summary>
	/// 生成消息预览
	/// </summary>
	/// <param name="groupUin">群号</param>
	/// <param name="entry">消息条目</param>
	Task<string> GenerateMessagePreviewAsync(ulong groupUin, GroupMessageEntry entry);

	/// <summary>
	/// 生成消息预览
	/// </summary>
	/// <param name="groupUin">群号</param>
	/// <param name="memberUin">QQ 号</param>
	/// <param name="message">消息</param>
	/// <returns></returns>
	Task<string> GenerateMessagePreviewAsync(ulong groupUin, ulong memberUin, Message message);
}
