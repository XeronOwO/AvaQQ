namespace AvaQQ.SDK;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ConfigurationNameAttribute(string name) : Attribute
{
	public string Name => name;
}
