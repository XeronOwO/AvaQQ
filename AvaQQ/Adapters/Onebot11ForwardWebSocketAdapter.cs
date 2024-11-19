using AvaQQ.SDK;
using AvaQQ.SDK.Logging;
using Makabaka;
using Makabaka.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AvaQQ.Adapters;

internal class Onebot11ForwardWebSocketAdapter : Adapter
{
	private readonly MakabakaApp _makabaka;

	private readonly TaskCompletionSource _connectCompletionSource = new();

	public override LogRecorder Logs { get; } = new();

	public override long Uin => _makabaka.BotContext.SelfId;

	public override async Task<string> GetNameAsync()
		=> (await _makabaka.BotContext.GetLoginInfoAsync()).Result.Nickname;

	public Onebot11ForwardWebSocketAdapter(string url, string accessToken)
	{
		var json = new
		{
			Bot = new
			{
				ForwardWebSocket = new
				{
					Enabled = true,
					Url = url,
					AccessToken = accessToken,
					ReconnectInterval = 0,
					ConnectionTimeout = 5000,
				},
			},
		};

		var builder = new MakabakaAppBuilder();
		builder.Services.ConfigureRecordLogger(Logs);
		builder.Configuration.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(json))));
		_makabaka = builder.Build();
	}

	public override async Task<bool> ConnectAsync(TimeSpan timeout)
	{
		_makabaka.BotContext.OnLifecycle += FirstOnLifecycle;
		_ = _makabaka.RunAsync();
		await Task.WhenAny(_connectCompletionSource.Task, Task.Delay(timeout));
		_makabaka.BotContext.OnLifecycle -= FirstOnLifecycle;

		if (!_connectCompletionSource.Task.IsCompletedSuccessfully)
		{
			await _makabaka.StopAsync();
			return false;
		}

		return true;
	}

	private Task FirstOnLifecycle(object sender, LifecycleEventArgs e)
	{
		if (e.SubType == LifecycleEventType.Connect)
		{
			_connectCompletionSource.SetResult();
		}

		return Task.CompletedTask;
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{

			}

			_makabaka.Dispose();
			disposedValue = true;
		}
	}

	~Onebot11ForwardWebSocketAdapter()
	{
		Dispose(disposing: false);
	}

	public override void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion
}
