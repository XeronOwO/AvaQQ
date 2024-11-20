using Avalonia.Controls;
using AvaQQ.SDK;
using AvaQQ.SDK.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
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
		try
		{
			if (!Directory.Exists(_logDirectory))
			{
				Directory.CreateDirectory(_logDirectory);
			}

			CompressLatestLog();
		}
		catch (Exception e)
		{
			Debug.WriteLine(e);
		}
	}

	private static void CompressLatestLog()
	{
		if (!File.Exists(_logFilePath))
		{
			return;
		}

		if (!Design.IsDesignMode)
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
			inputStream.Close();
		}

		File.Delete(_logFilePath);
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
				SimpleLogFormatter.Format(
					name,
					DateTime.Now,
					logLevel,
					eventId,
					state,
					exception,
					formatter
				)
			);
		}
		catch
		{
		}
	}
}
