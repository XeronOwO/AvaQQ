using System.Net.Http;

namespace AvaQQ.Resources;

internal static class Shared
{
	public static HttpClient HttpClient { get; } = new();
}
