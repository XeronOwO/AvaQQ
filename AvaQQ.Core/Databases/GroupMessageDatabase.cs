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
	/// 同步群消息
	/// </summary>
	/// <param name="groupUin">群号</param>
	/// <param name="entries">条目</param>
	public abstract void Sync(ulong groupUin, IEnumerable<GroupMessageEntry> entries);
}
