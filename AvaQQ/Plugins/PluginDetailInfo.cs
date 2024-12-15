namespace AvaQQ.Plugins;

internal class PluginDetailInfo
{
	public string Id { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public string[] Authors { get; set; } = [];

	public Version Version { get; set; } = new();

	public string[] Assemblies { get; set; } = [];

	public PluginDependencyInfo[] Dependencies { get; set; } = [];
}
