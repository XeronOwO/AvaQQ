﻿using AvaQQ.SDK;

namespace Onebot11ForwardWebSocketAdapter;

[ConfigurationName("onebot_11_forward_web_socket.json")]
internal class AdapterConfiguration
{
	public string Url { get; set; } = "ws://127.0.0.1:8081";

	public string AccessToken { get; set; } = string.Empty;
}
