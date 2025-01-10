﻿namespace AvaQQ.Core.Messages;

/// <summary>
/// 市场表情段
/// </summary>
public class MarketFaceSegment : Segment
{
	/// <summary>
	/// URL
	/// </summary>
	public string Url { get; set; } = string.Empty;

	/// <summary>
	/// 表情包 ID
	/// </summary>
	public ulong EmojiPackageId { get; set; }

	/// <summary>
	/// 表情 ID
	/// </summary>
	public string EmojiId { get; set; } = string.Empty;

	/// <summary>
	/// 密钥
	/// </summary>
	public string Key { get; set; } = string.Empty;

	/// <summary>
	/// 简介
	/// </summary>
	public string Summary { get; set; } = string.Empty;
}
