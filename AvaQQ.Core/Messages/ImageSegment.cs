namespace AvaQQ.Core.Messages;

/// <summary>
/// 图片片段
/// </summary>
/// <param name="Filename">图片文件名</param>
/// <param name="Url">图片 URL</param>
/// <param name="SubType">子类型</param>
/// <param name="Summary">摘要</param>
public record class ImageSegment(string Filename, string Url, int SubType, string Summary) : ISegment
{
	/// <inheritdoc/>
	public string Preview => Summary;
}
