namespace AvaQQ.SDK;

public interface IPluginManager
{
	void PreloadPlugins();

	void LoadPlugins();

	void PostLoadPlugins();

	void UnloadPlugins();
}
