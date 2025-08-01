namespace AvaQQ.Adapters.Lagrange.Utils;

internal static class TaskExtensions
{
	public static async Task<T> WithCancellationToken<T>(
		this Task<T> task,
		CancellationToken token)
	{
		using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
		Task delayTask = Task.Delay(Timeout.Infinite, linkedCts.Token);

		Task completedTask = await Task.WhenAny(task, delayTask);
		if (completedTask == task)
		{
			return await task;
		}
		else
		{
			linkedCts.Cancel();
			throw new OperationCanceledException(linkedCts.Token);
		}
	}
}
