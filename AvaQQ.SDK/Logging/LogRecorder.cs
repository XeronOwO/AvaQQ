using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace AvaQQ.SDK.Logging;

/// <summary>
/// 日志记录器
/// </summary>
public class LogRecorder
{
	/// <summary>
	/// 记录信息
	/// </summary>
	/// <param name="Name">日志器名称</param>
	/// <param name="Time">时间</param>
	/// <param name="LogLevel">日志等级</param>
	/// <param name="EventId">事件 ID</param>
	/// <param name="State">状态</param>
	/// <param name="Exception">异常</param>
	/// <param name="Message">信息</param>
	public record class RecordInfo(
		string Name,
		DateTime Time,
		LogLevel LogLevel,
		EventId EventId,
		object? State,
		Exception? Exception,
		string Message
		)
	{
		/// <summary>
		/// 格式化为字符串
		/// </summary>
		/// <returns>字符串</returns>
		public override string ToString()
			=> SimpleLogFormatter.Format(Name, Time, LogLevel, EventId, State, Exception, Message);
	}

	/// <summary>
	/// 记录
	/// </summary>
	public ConcurrentQueue<RecordInfo> Records { get; } = [];

	/// <summary>
	/// 记录一条日志
	/// </summary>
	/// <typeparam name="TState">状态</typeparam>
	/// <param name="name">日志器名称</param>
	/// <param name="time">时间</param>
	/// <param name="logLevel">日志等级</param>
	/// <param name="eventId">事件 ID</param>
	/// <param name="state">状态</param>
	/// <param name="exception">异常</param>
	/// <param name="formatter">格式化器</param>
	public void Record<TState>(
		string name,
		DateTime time,
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
	{
		Records.Enqueue(
			new RecordInfo(
				name,
				time,
				logLevel,
				eventId,
				state,
				exception,
				formatter(state, exception)
			)
		);
	}

	/// <summary>
	/// 格式化为字符串
	/// </summary>
	/// <returns>字符串</returns>
	public override string ToString()
	{
		var sb = new StringBuilder();
		foreach (var record in Records)
		{
			sb.Append(record.ToString());
		}
		return sb.ToString();
	}
}
