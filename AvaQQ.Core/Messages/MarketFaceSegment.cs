namespace AvaQQ.Core.Messages;

/// <summary>
/// 市场表情段
/// </summary>
/// <param name="Url">URL</param>
/// <param name="EmojiPackageId">表情包 ID</param>
/// <param name="EmojiId">表情 ID</param>
/// <param name="Key">密钥</param>
/// <param name="Summary">简介</param>
public record class MarketFaceSegment(
	string Url, ulong EmojiPackageId, string EmojiId, string Key, string Summary
	) : ISegment
{
	/// <inheritdoc/>
	public string Preview => Summary;
}
