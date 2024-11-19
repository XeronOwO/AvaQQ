namespace AvaQQ.Configurations;

internal class ConnectConfiguration
{
	public int AdapterIndex { get; set; }

	public class Onebot11ForwardWebSocketInfo
	{
		public string Url { get; set; } = "ws://127.0.0.1:8081";

		public string AccessToken { get; set; } = string.Empty;
	}

	public Onebot11ForwardWebSocketInfo Onebot11ForwardWebSocket { get; set; } = new();
}
