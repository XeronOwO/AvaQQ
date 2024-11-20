using System;

namespace AvaQQ.SDK;

/// <summary>
/// 配置名称特性
/// </summary>
/// <param name="name">配置文件名称，记得加 .json 后缀</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ConfigurationNameAttribute(string name) : Attribute
{
	/// <summary>
	/// 配置文件名称
	/// </summary>
	public string Name => name;
}
