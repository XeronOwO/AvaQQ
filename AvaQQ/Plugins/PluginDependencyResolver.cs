using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace AvaQQ.Plugins;

internal class PluginDependencyResolver(
	string id,
	string directory,
	PluginDependencyResolver[] fallbacks)
{
	public PluginDependencyResolver(string id, string directory)
		: this(id, directory, [])
	{
	}

	public Assembly? Resolve(AssemblyLoadContext context, AssemblyName name)
	{
		var path = Path.Combine(directory, $"{name.Name}.dll");
		if (File.Exists(path))
		{
			return context.LoadFromAssemblyPath(path);
		}

		foreach (var fallback in fallbacks)
		{
			if (fallback.Resolve(context, name) is Assembly assembly)
			{
				return assembly;
			}
		}

		return null;
	}

	public override string ToString()
	{
		return $"[{id}] {directory}";
	}
}
