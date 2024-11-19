using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace AvaQQ.SDK.Logging;

/// <summary>
/// A logger that records log messages.
/// </summary>
public class LogRecorder
{
	/// <summary>
	/// Represents a log record.
	/// </summary>
	/// <param name="Name">The name of the logger.</param>
	/// <param name="LogLevel">The log level of the message.</param>
	/// <param name="EventId">The event ID of the message.</param>
	/// <param name="State">The state of the message.</param>
	/// <param name="Exception">The exception of the message.</param>
	/// <param name="Message">The message.</param>
	public record class RecordInfo(
		string Name,
		LogLevel LogLevel,
		EventId EventId,
		object? State,
		Exception? Exception,
		string Message
		);

	/// <summary>
	/// The records of the logger.
	/// </summary>
	public ConcurrentQueue<RecordInfo> Records { get; } = [];

	/// <summary>
	/// Records a log message.
	/// </summary>
	/// <typeparam name="TState"></typeparam>
	/// <param name="name">The name of the logger.</param>
	/// <param name="logLevel">The log level of the message.</param>
	/// <param name="eventId">The event ID of the message.</param>
	/// <param name="state">The state of the message.</param>
	/// <param name="exception">The exception of the message.</param>
	/// <param name="formatter">The formatter of the message.</param>
	public void Record<TState>(
		string name,
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
	{
		Records.Enqueue(new RecordInfo(name, logLevel, eventId, state, exception, formatter(state, exception)));
	}
}
