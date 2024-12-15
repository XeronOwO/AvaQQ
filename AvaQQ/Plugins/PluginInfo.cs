using AvaQQ.SDK;
using System.Text.Json.Serialization;

namespace AvaQQ.Plugins;

internal class PluginInfo
{
	public bool Enabled { get; set; } = true;

	[JsonIgnore]
	public bool Visited { get; set; } = false;

	[JsonIgnore]
	public string Directory { get; set; } = string.Empty;

	[JsonIgnore]
	public PluginDetailInfo Detail { get; set; } = new();

	[JsonIgnore]
	public Plugin[] PluginInstances { get; set; } = [];
}

internal class PluginInfos : Dictionary<string, PluginInfo>
{
}
