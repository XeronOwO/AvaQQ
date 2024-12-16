using AvaQQ.SDK;

namespace AvaQQ.Adapters.Onebot11ForwardWebSocket;

[ConfigurationName("onebot_11_forward_web_socket.json")]
internal class AdapterConfiguration
{
	public string Url { get; set; } = "ws://127.0.0.1:8081";

	public string AccessToken { get; set; } = string.Empty;

	public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(10);
}
