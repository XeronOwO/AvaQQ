using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Svg.Skia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;

namespace AvaQQ.Desktop;

internal class Program
{
	// Initialization code. Don't use any Avalonia, third-party APIs or any
	// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
	// yet and stuff might break.
	[STAThread]
	public static void Main(string[] args)
		=> BuildAvaloniaApp(args).Start(AppMain, args);

	// Avalonia configuration, don't remove; also used by visual designer.
	public static AppBuilder BuildAvaloniaApp(string[] args)
	{
		// https://github.com/wieslawsoltes/Svg.Skia?tab=readme-ov-file#avalonia-previewer
		// To make svg controls work with Avalonia Previewer
		GC.KeepAlive(typeof(SvgImageExtension).Assembly);
		GC.KeepAlive(typeof(Avalonia.Svg.Skia.Svg).Assembly);

		return AppBuilder.Configure<App>()
			.UsePlatformDetect()
			.WithInterFont()
			.LogToTrace()
			.UseReactiveUI();
	}

	private static readonly CancellationTokenSource _cts = new();

	private static void AppMain(Application app, string[] args)
	{
		if (app is App app1)
		{
			app1.LifetimeCTS = _cts;
			app1.Run(_cts.Token);
		}
	}
}
