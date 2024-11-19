using System.Text.Json.Serialization;

namespace AvaQQ.Plugins;

internal class PluginStatusInfo
{
	public bool Enabled { get; set; } = true;

	[JsonIgnore]
	public bool Visited { get; set; } = false;
}
