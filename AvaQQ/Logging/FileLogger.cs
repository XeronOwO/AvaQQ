using AvaQQ.SDK;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;

namespace AvaQQ.Logging;

internal class FileLogger(
	string name,
	Func<FileLoggerConfiguration> getCurrentConfig) : ILogger
{
	private static readonly string _logDirectory = Path.Combine(
		Constants.StorageDirectory,
		"logs"
	);

	private static readonly string _logFilePath = Path.Combine(
		_logDirectory,
		"latest.log"
	);

	static FileLogger()
	{
		if (!Directory.Exists(_logDirectory))
		{
			Directory.CreateDirectory(_logDirectory);
		}

		CompressLatestLog();
	}

	private static void CompressLatestLog()
	{
		if (!File.Exists(_logFilePath))
		{
			return;
		}

		try
		{
			var creationTime = File.GetCreationTime(_logFilePath);
			var outputFilePath = Path.Combine(
				_logDirectory,
				$"{creationTime:yyyy-MM-dd-HH-mm-ss}.log.gz"
			);
			using var outputStream = new FileStream(outputFilePath, FileMode.Create);
			using var gZipStream = new GZipStream(outputStream, CompressionLevel.SmallestSize);
			using var inputStream = new FileStream(_logFilePath, FileMode.Open);
			inputStream.CopyTo(gZipStream);

			File.Delete(_logFilePath);
		}
		catch (Exception)
		{
		}
	}

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		=> default;

	public bool IsEnabled(LogLevel logLevel)
		=> logLevel >= getCurrentConfig().LogLevel;

	public void Log<TState>(
		LogLevel logLevel,
		EventId eventId,
		TState state,
		Exception? exception,
		Func<TState, Exception?, string> formatter)
	{
		if (!IsEnabled(logLevel))
		{
			return;
		}

		try
		{
			File.AppendAllText(
				_logFilePath,
				$"{GetLogLevelString(logLevel)}: {name}[{eventId.Id}] @{DateTime.Now}{Environment.NewLine}" +
				$"      {formatter(state, exception)}{Environment.NewLine}"
			);
		}
		catch (Exception)
		{
		}
	}

	private static string GetLogLevelString(LogLevel logLevel)
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
