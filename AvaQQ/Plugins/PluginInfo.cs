using System.Collections.Generic;

namespace AvaQQ.Plugins;

internal class PluginInfo
{
	public string Id { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string[] Assemblies { get; set; } = [];
}

internal class PluginInfos : Dictionary<string, PluginInfo>
{
}
