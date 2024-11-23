using System;

namespace AvaQQ;

internal enum MediaType
{
	Unknown,
	Jpeg,
	Png,
	Bmp,
	Gif,
	Tiff,
	WebP,
}

internal static class MediaTypeExtensions
{
	private static readonly byte[] _jpegHeader = [0xFF, 0xD8];

	private static readonly byte[] _pngHeader = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

	private static readonly byte[] _bmpHeader = [0x42, 0x4D];

	private static readonly byte[] _gifHeader1 = [0x47, 0x49, 0x46, 0x38, 0x37, 0x61];

	private static readonly byte[] _gifHeader2 = [0x47, 0x49, 0x46, 0x38, 0x39, 0x61];

	private static readonly byte[] _tiffHeader1 = [0x4D, 0x4D];

	private static readonly byte[] _tiffHeader2 = [0x49, 0x49];

	private static readonly byte[] _webPHeader = [(byte)'R', (byte)'I', (byte)'F', (byte)'F'];

	private static bool StartsWith(this byte[] bytes, byte[] header)
	{
		if (bytes.Length < header.Length)
		{
			return false;
		}
		for (var i = 0; i < header.Length; i++)
		{
			if (bytes[i] != header[i])
			{
				return false;
			}
		}
		return true;
	}

	public static MediaType GetMediaType(this byte[] bytes)
	{
		if (bytes.StartsWith(_jpegHeader))
		{
			return MediaType.Jpeg;
		}

		if (bytes.StartsWith(_pngHeader))
		{
			return MediaType.Png;
		}

		if (bytes.StartsWith(_bmpHeader))
		{
			return MediaType.Bmp;
		}

		if (bytes.StartsWith(_gifHeader1) || bytes.StartsWith(_gifHeader2))
		{
			return MediaType.Gif;
		}

		if (bytes.StartsWith(_tiffHeader1) || bytes.StartsWith(_tiffHeader2))
		{
			return MediaType.Tiff;
		}

		if (bytes.StartsWith(_webPHeader))
		{
			return MediaType.WebP;
		}

		return MediaType.Unknown;
	}

	public static bool IsImage(this MediaType mediaType)
	{
		return mediaType switch
		{
			MediaType.Jpeg => true,
			MediaType.Png => true,
			MediaType.Bmp => true,
			MediaType.Gif => true,
			MediaType.Tiff => true,
			MediaType.WebP => true,
			_ => throw new NotImplementedException(),
		};
	}

	public static string GetFileExtension(this MediaType mediaType)
	{
		return mediaType switch
		{
			MediaType.Jpeg => ".jpg",
			MediaType.Png => ".png",
			MediaType.Bmp => ".bmp",
			MediaType.Gif => ".gif",
			MediaType.Tiff => ".tiff",
			MediaType.WebP => ".webp",
			_ => throw new NotImplementedException(),
		};
	}
}
