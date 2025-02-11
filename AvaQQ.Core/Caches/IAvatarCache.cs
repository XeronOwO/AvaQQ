﻿using Avalonia.Media.Imaging;

namespace AvaQQ.Core.Caches;

/// <summary>
/// 头像缓存
/// </summary>
public interface IAvatarCache
{
	/// <summary>
	/// 获取群头像
	/// </summary>
	/// <param name="uin">群号</param>
	/// <param name="size">
	/// 头像尺寸<br/>
	/// 仅限官方支持的尺寸，例如 0、40、100、160、640，其中 0 是原图<br/>
	/// 240可以获取默认头像（可能是官方BUG or 特性？）
	/// </param>
	/// <param name="noCache">不使用缓存</param>
	Task<Bitmap?> GetGroupAvatarAsync(ulong uin, int size = 0, bool noCache = false);

	/// <summary>
	/// 获取用户头像
	/// </summary>
	/// <param name="uin">QQ 号</param>
	/// <param name="size">
	/// 头像尺寸<br/>
	/// 仅限官方支持的尺寸，例如 0、40、100、160、640，其中 0 是原图<br/>
	/// 240可以获取默认头像（可能是官方BUG or 特性？）
	/// </param>
	/// <param name="noCache">不使用缓存</param>
	Task<Bitmap?> GetUserAvatarAsync(ulong uin, int size = 0, bool noCache = false);

	/// <summary>
	/// 只移除引用，实际释放内存由 GC 完成
	/// </summary>
	void ClearGroupAvatars();
}
