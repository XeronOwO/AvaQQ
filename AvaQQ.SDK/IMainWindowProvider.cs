using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace AvaQQ.SDK;

public interface IMainWindowProvider
{
	IServiceScope? Scope { get; }

	[NotNullIfNotNull(nameof(Scope))]
	Window? Window { get; }

	void OpenOrActivateMainWindow();
}
