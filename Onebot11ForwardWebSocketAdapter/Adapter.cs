using AvaQQ.SDK.Adapters;
using AvaQQ.SDK.Logging;
using Makabaka;
using Makabaka.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Onebot11ForwardWebSocketAdapter;

internal class Adapter : IAdapter
{
	private readonly IServiceProvider _serviceProvider;

	private readonly MakabakaApp _makabaka;

	private readonly ILogger<Adapter> _logger;

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

	private readonly TaskCompletionSource _connectCompletionSource = new();

	private readonly LogRecorder _logRecorder = new();

	public Adapter(IServiceProvider serviceProvider, string url, string accessToken)
	{
		_serviceProvider = serviceProvider;

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
		_logger = serviceProvider.GetRequiredService<ILogger<Adapter>>();
	}

	public long Uin => _makabaka.BotContext.SelfId;

	public async Task<string> GetNicknameAsync()
	{
		try
		{
			return (await _makabaka.BotContext.GetLoginInfoAsync()).Result.Nickname;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get nickname.");
			return string.Empty;
		}
	}

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

	public async Task<IEnumerable<BriefFriendInfo>> GetFriendListAsync()
	{
		try
		{
			var friends = (await _makabaka.BotContext.GetFriendListAsync()).Result;
			return friends.Select(f => new BriefFriendInfo()
			{
				Uin = f.UserId,
				Nickname = f.Nickname,
				Remark = f.Remark,
			});
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get friend list.");
			return [];
		}
	}
}
