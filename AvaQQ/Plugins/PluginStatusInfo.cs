using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AvaQQ.Plugins;

internal class PluginStatusInfo
{
	public bool Enabled { get; set; } = true;

	[JsonIgnore]
	public bool Visited { get; set; } = false;
}

internal class PluginStatusInfos : Dictionary<string, PluginStatusInfo>
{
}
