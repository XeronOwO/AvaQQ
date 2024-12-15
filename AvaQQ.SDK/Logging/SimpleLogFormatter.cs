using Microsoft.Extensions.Logging;
using System.Text;

namespace AvaQQ.SDK.Logging;

/// <summary>
/// 日志格式化器
/// </summary>
public static class SimpleLogFormatter
{
	/// <summary>
	/// 格式化日志
	/// </summary>
	/// <typeparam name="TState">状态</typeparam>
	/// <param name="name">日志器名称</param>
	/// <param name="time">时间</param>
	/// <param name="logLevel">日志等级</param>
	/// <param name="eventId">事件 ID</param>
	/// <param name="state">状态</param>
	/// <param name="exception">异常</param>
	/// <param name="formatter">格式化器</param>
	/// <returns>格式化字符串</returns>
	public static string Format<TState>(
		string name,
		DateTime time,
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
		=> Format(name, time, logLevel, eventId, state, exception, formatter(state, exception));

	/// <summary>
	/// 格式化日志
	/// </summary>
	/// <typeparam name="TState">状态</typeparam>
	/// <param name="name">日志器名称</param>
	/// <param name="time">时间</param>
	/// <param name="logLevel">日志等级</param>
	/// <param name="eventId">事件 ID</param>
	/// <param name="state">状态</param>
	/// <param name="exception">异常</param>
	/// <param name="message">消息</param>
	/// <returns>格式化字符串</returns>
	public static string Format<TState>(
		string name,
		DateTime time,
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		string message)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{GetLogLevelString(logLevel)}: {name}[{eventId.Id}] @ {time}");
		sb.AppendLine($"      {message}");
		if (exception is not null)
		{
			sb.AppendLine(exception.ToString());
		}

		return sb.ToString();
	}

	/// <summary>
	/// 获取日志等级字符串
	/// </summary>
	/// <param name="logLevel">日志等级</param>
	/// <returns>日志等级字符串</returns>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public static string GetLogLevelString(LogLevel logLevel)
	{
		return logLevel switch
		{
			LogLevel.Trace => "trce",
			LogLevel.Debug => "dbug",
			LogLevel.Information => "info",
			LogLevel.Warning => "warn",
			LogLevel.Error => "fail",
			LogLevel.Critical => "crit",
			_ => throw new ArgumentOutOfRangeException(nameof(logLevel))
		};
	}
}
