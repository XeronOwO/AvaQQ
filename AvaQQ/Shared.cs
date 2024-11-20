using System.Net.Http;

namespace AvaQQ;

internal static class Shared
{
	public static HttpClient HttpClient { get; } = new();
}
