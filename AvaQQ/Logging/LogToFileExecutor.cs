using Avalonia.Controls;
using AvaQQ.SDK;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text;

namespace AvaQQ.Logging;

internal class LogToFileExecutor : IDisposable
{
	private static readonly string _logDirectory = Path.Combine(
		Constants.RootDirectory,
		"logs"
	);

	private static readonly string _logPath = Path.Combine(
		_logDirectory,
		"latest.log"
	);

	private readonly IConfiguration _configuration;

	private readonly CancellationTokenSource _cts = new();

	public LogToFileExecutor(IConfiguration configuration)
	{
		_configuration = configuration;

		if (Design.IsDesignMode)
		{
			return;
		}

		Directory.CreateDirectory(_logDirectory);

		CompressLatestLog();
		DeleteOldLogs();
		StartLogToFileLoop();
	}

	private static void CompressLatestLog()
	{
		if (Design.IsDesignMode)
		{
			return;
		}
		if (!File.Exists(_logPath))
		{
			return;
		}

		var creationTime = File.GetCreationTime(_logPath);
		var outputFilePath = Path.Combine(
			_logDirectory,
			$"{creationTime:yyyy-MM-dd-HH-mm-ss}.log.gz"
		);
		using var outputStream = new FileStream(outputFilePath, FileMode.Create);
		using var gZipStream = new GZipStream(outputStream, CompressionLevel.SmallestSize);
		using var inputStream = new FileStream(_logPath, FileMode.Open);
		inputStream.CopyTo(gZipStream);
		inputStream.Close();
		File.Delete(_logPath);
	}

	private void DeleteOldLogs()
	{
		if (!Directory.Exists(_logDirectory))
		{
			return;
		}

		var files = Directory.GetFiles(_logDirectory, "*.log.gz").ToList();
		files.Sort((file1, file2) =>
		{
			var time1 = File.GetCreationTime(file1);
			var time2 = File.GetCreationTime(file2);
			return -time1.CompareTo(time2);
		});

		var maxFileCount = _configuration.GetValue("Logging:MaxLogFileCount", 10);
		for (var i = maxFileCount; i < files.Count; i++)
		{
			File.Delete(files[i]);
		}
	}

	public void Append(string formattedText)
	{
		if (disposedValue || Design.IsDesignMode)
		{
			return;
		}

		_logQueue.Add(formattedText);
	}

	private readonly BlockingCollection<string> _logQueue = [];

	private void StartLogToFileLoop()
		=> Task.Run(() =>
		{
			using var fileStream = new FileStream(
				_logPath,
				FileMode.Append,
				FileAccess.Write,
				FileShare.Read
			);

			while (!_cts.Token.IsCancellationRequested)
			{
				try
				{
					var log = _logQueue.Take(_cts.Token);
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
		}, _cts.Token);

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_cts.Cancel();

			if (disposing)
			{
				_cts.Dispose();
				_logQueue.Dispose();
			}

			disposedValue = true;
		}
	}

	~LogToFileExecutor()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion
}
