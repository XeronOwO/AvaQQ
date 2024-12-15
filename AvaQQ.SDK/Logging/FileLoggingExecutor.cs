using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text;
using Config = AvaQQ.SDK.Configuration<AvaQQ.SDK.Configurations.LogConfiguration>;

namespace AvaQQ.SDK.Logging;

/// <summary>
/// 文件日志执行器<br/>
/// 非必要情况下不要直接使用此类，推荐使用 <see cref="ILogger"/> 记录日志。
/// </summary>
public class FileLoggingExecutor
{
	private static readonly string _logDirectory = Path.Combine(
		Constants.RootDirectory,
		"logs"
	);

	private static readonly string _logFilePath = Path.Combine(
		_logDirectory,
		"latest.log"
	);

	private static readonly CancellationTokenSource _cts = new();

	static FileLoggingExecutor()
	{
		if (!Directory.Exists(_logDirectory))
		{
			Directory.CreateDirectory(_logDirectory);
		}

		CompressLatestLog();
		DeleteOldLogs();
		StartLogToFileLoop(_cts.Token);
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

	private static void DeleteOldLogs()
	{
		if (!Directory.Exists(_logDirectory))
		{
			return;
		}

		var files = Directory.GetFiles(_logDirectory, "*.log.gz").ToList();
		files.Sort((a, b) =>
		{
			var aTime = File.GetCreationTime(a);
			var bTime = File.GetCreationTime(b);
			return -aTime.CompareTo(bTime);
		});

		for (int i = Config.Instance.MaxFileCount; i < files.Count; i++)
		{
			File.Delete(files[i]);
		}
	}

	private static readonly BlockingCollection<string> _logQueue = [];

	private static void StartLogToFileLoop(CancellationToken token)
	{
		Task.Run(() =>
		{
			using var fileStream = new FileStream(
				_logFilePath,
				FileMode.Append,
				FileAccess.Write,
				FileShare.Read
			);

			while (!token.IsCancellationRequested)
			{
				try
				{
					var log = _logQueue.Take(token);
					fileStream.Write(Encoding.UTF8.GetBytes(log));
					fileStream.Flush();
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch
				{
				}
			}
		}, token);
	}

	/// <summary>
	/// 追加文本到日志文件
	/// </summary>
	/// <param name="formattedText">已格式化的文本</param>
	public static void Append(string formattedText)
		=> _logQueue.Add(formattedText);

	/// <summary>
	/// 记录跟踪日志
	/// </summary>
	public static void Log<T>(LogLevel logLevel, Exception? exception, string message)
		=> Append(SimpleLogFormatter.Format(
			typeof(T).FullName ?? string.Empty,
			DateTime.Now,
			logLevel,
			new EventId(0),
			message,
			exception,
			message
		));

	/// <summary>
	/// 记录跟踪日志
	/// </summary>
	public static void Trace<T>(string message)
		=> Trace<T>(null, message);

	/// <summary>
	/// 记录跟踪日志
	/// </summary>
	public static void Trace<T>(Exception? exception, string message)
		=> Log<T>(LogLevel.Trace, exception, message);

	/// <summary>
	/// 记录调试日志
	/// </summary>
	public static void Debug<T>(string message)
		=> Debug<T>(null, message);

	/// <summary>
	/// 记录调试日志
	/// </summary>
	public static void Debug<T>(Exception? exception, string message)
		=> Log<T>(LogLevel.Debug, exception, message);

	/// <summary>
	/// 记录信息日志
	/// </summary>
	public static void Information<T>(string message)
		=> Information<T>(null, message);

	/// <summary>
	/// 记录信息日志
	/// </summary>
	public static void Information<T>(Exception? exception, string message)
		=> Log<T>(LogLevel.Information, exception, message);

	/// <summary>
	/// 记录警告日志
	/// </summary>
	public static void Warning<T>(string message)
		=> Warning<T>(null, message);

	/// <summary>
	/// 记录警告日志
	/// </summary>
	public static void Warning<T>(Exception? exception, string message)
		=> Log<T>(LogLevel.Warning, exception, message);

	/// <summary>
	/// 记录错误日志
	/// </summary>
	public static void Error<T>(string message)
		=> Error<T>(null, message);

	/// <summary>
	/// 记录错误日志
	/// </summary>
	public static void Error<T>(Exception? exception, string message)
		=> Log<T>(LogLevel.Error, exception, message);

	/// <summary>
	/// 记录错误日志
	/// </summary>
	public static void Critical<T>(string message)
		=> Critical<T>(null, message);

	/// <summary>
	/// 记录错误日志
	/// </summary>
	public static void Critical<T>(Exception? exception, string message)
		=> Log<T>(LogLevel.Critical, exception, message);

	/// <summary>
	/// 释放资源
	/// </summary>
	public static void Dispose()
	{
		_cts.Cancel();
		_cts.Dispose();
		_logQueue.Dispose();
	}
}
