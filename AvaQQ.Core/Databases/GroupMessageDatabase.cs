using AvaQQ.Core.Adapters;

namespace AvaQQ.Core.Databases;

/// <summary>
/// 群消息数据库
/// </summary>
public abstract class GroupMessageDatabase : Database
{
	/// <summary>
	/// 插入群消息到数据库
	/// </summary>
	/// <param name="groupUin">群号</param>
	/// <param name="entry">条目</param>
	public abstract void Insert(ulong groupUin, GroupMessageEntry entry);

	/// <summary>
	/// 获取最后一条群消息
	/// </summary>
	/// <param name="groupUin">群号</param>
	public abstract GroupMessageEntry? Latest(ulong groupUin);

	/// <summary>
	/// 当收到群消息时触发
	/// </summary>
	public void Adapter_OnGroupMessage(object? sender, GroupMessageEventArgs e)
	{
		Insert(e.GroupUin, e);
	}
}
