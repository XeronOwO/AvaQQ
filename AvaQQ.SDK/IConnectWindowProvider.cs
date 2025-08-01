using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace AvaQQ.SDK;

public interface IConnectWindowProvider
{
	IServiceScope? Scope { get; }

	[NotNullIfNotNull(nameof(Scope))]
	ConnectWindowBase? Window { get; }

	void OpenConnectWindow();
}
