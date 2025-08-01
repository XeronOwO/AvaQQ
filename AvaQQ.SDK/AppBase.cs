using Avalonia;
using AvaQQ.SDK.Resources;

namespace AvaQQ.SDK;

public abstract class AppBase : Application
{
	public abstract IServiceProvider Services { get; }

	public static new AppBase Current
	{
		get
		{
			if (Application.Current is not { } app)
			{
				throw new InvalidOperationException(SR.ExceptionCurrentApplicationIsNull);
			}
			if (app is not AppBase appBase)
			{
				throw new InvalidOperationException(SR.ExceptionUnsupportedApplicationType);
			}
			return appBase;
		}
	}
}
