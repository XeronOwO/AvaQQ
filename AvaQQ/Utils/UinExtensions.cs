using Avalonia.Media.Imaging;
using System;
using System.Threading.Tasks;

namespace AvaQQ.Utils;

internal static class UinExtensions
{
	public static string GetAvatarUrl(this long uin, int size = 0)
	{
		return $"https://q1.qlogo.cn/g?b=qq&nk={uin}&s={size}";
	}

	public static async Task<Bitmap?> GetAvatarImageAsync(this long uin, int size = 0)
	{
		try
		{
			var url = uin.GetAvatarUrl(size);
			var response = await Shared.HttpClient.GetAsync(url);
			response.EnsureSuccessStatusCode();
			using var stream = await response.Content.ReadAsStreamAsync();

			return new Bitmap(stream);
		}
		catch (Exception)
		{
			return null;
		}
	}
}
