using AvaQQ.SDK.Adapters;
using System;

namespace AvaQQ.SDK.Databases;

/// <summary>
/// 群消息数据库
/// </summary>
public abstract class GroupMessageDatabase : Database
{
	/// <inheritdoc/>
	public override void Initialize()
	{
		if (AppBase.Current.Adapter is not { } adapter)
		{
			throw new InvalidOperationException("Adapter is null");
		}

		adapter.OnGroupMessage += Adapter_OnGroupMessage;
	}

	/// <summary>
	/// 插入群消息到数据库
	/// </summary>
	/// <param name="entry">条目</param>
	public abstract void Insert(GroupMessageEntry entry);

	private void Adapter_OnGroupMessage(object? sender, GroupMessageEventArgs e)
	{
		Insert(e);
	}
}
