namespace AvaQQ.Core.Messages;

/// <summary>
/// 图片片段
/// </summary>
public class ImageSegment : Segment
{
	/// <summary>
	/// 图片文件名
	/// </summary>
	public string Filename { get; set; } = string.Empty;

	/// <summary>
	/// 图片 URL
	/// </summary>
	public string Url { get; set; } = string.Empty;

	/// <summary>
	/// 子类型
	/// </summary>
	public int SubType { get; set; }

	/// <summary>
	/// 摘要
	/// </summary>
	public string Summary { get; set; } = string.Empty;
}
