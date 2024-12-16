namespace AvaQQ.Core.Utils;

/// <summary>
/// 循环注入监测器<br/>
/// 能够检测出循环注入的情况，并抛出异常
/// </summary>
/// <typeparam name="T">要监视的类型</typeparam>
public static class CirculationInjectionDetector<T>
{
	private static int _count;

	/// <summary>
	/// 进入
	/// </summary>
	/// <exception cref="InvalidOperationException"/>
	public static void Enter()
	{
		var original = Interlocked.CompareExchange(ref _count, 1, 0);
		if (original == 1)
		{
			throw new InvalidOperationException($"Circulation injection detected: {typeof(T).FullName}.");
		}
	}

	/// <summary>
	/// 离开
	/// </summary>
	public static void Leave()
	{
		Interlocked.Exchange(ref _count, 0);
	}
}
