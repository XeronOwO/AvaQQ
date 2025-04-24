using AvaQQ.SDK;

namespace AvaQQ.Core.Databases;

/// <summary>
/// 数据库
/// </summary>
public abstract class Database : IDisposable
{
	/// <summary>
	/// 数据库目录
	/// </summary>
	public static string BaseDirectory { get; } = Path.Combine(Constants.RootDirectory, "db");

	static Database()
	{
		Directory.CreateDirectory(BaseDirectory);
	}

	/// <summary>
	/// 初始化数据库
	/// </summary>
	/// <param name="uin">用户 ID</param>
	public abstract void Initialize(ulong uin);

	/// <summary>
	/// 获取所有记录的群聊信息
	/// </summary>
	/// <param name="token">取消令牌</param>
	public abstract Task<RecordedGroupInfo[]> GetAllRecordedGroupsAsync(CancellationToken token = default);

	/// <summary>
	/// 获取所有记录的用户信息
	/// </summary>
	/// <param name="token">取消令牌</param>
	public abstract Task<RecordedUserInfo[]> GetAllRecordedUsersAsync(CancellationToken token = default);

	/// <inheritdoc/>
	public abstract void Dispose();
}
