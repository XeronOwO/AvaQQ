using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using AvaQQ.Exceptions;
using AvaQQ.Resources;
using System.Diagnostics.CodeAnalysis;

namespace AvaQQ;

/// <summary>
/// 防止设计器崩溃，请勿在运行时使用。
/// </summary>
internal static class DesignerHelper
{
	private static IServiceProvider? _services;

	[AllowNull]
	public static IServiceProvider Services
	{
		get
		{
			if (!Design.IsDesignMode)
			{
				throw new InvalidOperationException(SR.ExceptionOnlySupportedInDesignMode);
			}
			if (_services is not { } root)
			{
				throw new NotInitializedException(nameof(Services));
			}
			return root;
		}
		set => _services = value;
	}

	public static void ShowGridBackground(UserControl control)
	{
		if (!Design.IsDesignMode)
		{
			return;
		}

		control.Styles.Add(new Style(x => x.OfType<Grid>())
		{
			Setters =
			{
				new Setter(Panel.BackgroundProperty, new SolidColorBrush(new Color(0x20, 0x00, 0x00, 0x00))),
			},
		});
	}
}
