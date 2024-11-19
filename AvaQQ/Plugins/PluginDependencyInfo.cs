using System;

namespace AvaQQ.Plugins;

internal class PluginDependencyInfo
{
	public string Id { get; set; } = string.Empty;

	public Version Version { get; set; } = new();
}
