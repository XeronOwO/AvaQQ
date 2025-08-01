using System.Reflection;

namespace AvaQQ.Utils;

internal static class MemberInfoExtensions
{
	public static string GetFullName(this MemberInfo member)
	{
		ArgumentNullException.ThrowIfNull(member);

		if (member.DeclaringType is not { } type)
		{
			return member.Name;
		}
		return type.FullName + "." + member.Name;
	}
}
