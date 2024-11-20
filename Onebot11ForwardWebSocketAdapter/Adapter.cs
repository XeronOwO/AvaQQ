using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.Logging;
using Makabaka;
using Makabaka.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Text.Json;

namespace Onebot11ForwardWebSocketAdapter;

internal class Adapter : IAdapter
{
	private readonly MakabakaApp _makabaka;

	private readonly TaskCompletionSource _connectCompletionSource = new();

	private readonly LogRecorder _logRecorder = new();

	public Adapter(string url, string accessToken)
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
		builder.Services.ConfigureRecordLogger(_logRecorder);
		builder.Configuration.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(json))));
		_makabaka = builder.Build();
	}

	public long Uin => _makabaka.BotContext.SelfId;

	public async Task<string> GetNicknameAsync()
		=> (await _makabaka.BotContext.GetLoginInfoAsync()).Result.Nickname;

	public async Task<(bool, LogRecorder)> TryConnectAsync(TimeSpan timeout)
	{
		_makabaka.BotContext.OnLifecycle += FirstOnLifecycle;
		_ = _makabaka.RunAsync();
		await Task.WhenAny(_connectCompletionSource.Task, Task.Delay(timeout));
		_makabaka.BotContext.OnLifecycle -= FirstOnLifecycle;

		if (!_connectCompletionSource.Task.IsCompletedSuccessfully)
		{
			await _makabaka.StopAsync();
			return (false, _logRecorder);
		}

		return (true, _logRecorder);
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

	~Adapter()
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
