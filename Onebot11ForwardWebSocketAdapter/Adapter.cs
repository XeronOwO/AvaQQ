using AvaQQ.SDK;
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
using AGroupMessageEventArgs = AvaQQ.SDK.Adapters.GroupMessageEventArgs;
using MGroupMessageEventArgs = Makabaka.Events.GroupMessageEventArgs;

namespace Onebot11ForwardWebSocketAdapter;

internal class Adapter : IAdapter
{
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

			_makabaka.StopAsync().Wait();
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

	private readonly IUserManager _friendManager;

	public Adapter(IServiceProvider serviceProvider, string url, string accessToken)
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
		builder.Services.ConfigureRecordLogger(_logRecorder, serviceProvider.GetRequiredService<ILoggerProvider>());
		builder.Configuration.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(json))));
		_makabaka = builder.Build();
		_logger = serviceProvider.GetRequiredService<ILogger<Adapter>>();
		_friendManager = serviceProvider.GetRequiredService<IUserManager>();
	}

	public ulong Uin => _makabaka.BotContext.SelfId;

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
		_logRecorder.Start();
		_makabaka.BotContext.OnLifecycle += FirstOnLifecycle;
		_ = _makabaka.RunAsync();
		await Task.WhenAny(_connectCompletionSource.Task, Task.Delay(timeout));
		_makabaka.BotContext.OnLifecycle -= FirstOnLifecycle;
		_logRecorder.Stop();

		if (!_connectCompletionSource.Task.IsCompletedSuccessfully)
		{
			await _makabaka.StopAsync();
			return (false, _logRecorder);
		}

		RegisterEvents();

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

	private void RegisterEvents()
	{
		_makabaka.BotContext.OnGroupMessage += BotContext_OnGroupMessage;
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

	public async Task<IEnumerable<BriefGroupInfo>> GetGroupListAsync()
	{
		try
		{
			var groups = (await _makabaka.BotContext.GetGroupListAsync()).Result;
			return groups.Select(f => new BriefGroupInfo()
			{
				Uin = f.GroupId,
				Name = f.GroupName,
				Remark = string.Empty,
			});
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get group list.");
			return [];
		}
	}

	public event EventHandler<AGroupMessageEventArgs>? OnGroupMessage;

	private async Task BotContext_OnGroupMessage(object sender, MGroupMessageEventArgs e)
	{
		if (OnGroupMessage is null)
		{
			return;
		}

		var isAnonymous = e.SubType == GroupMessageEventType.Anonymous;

		string senderRemark;
		if (isAnonymous)
		{
			senderRemark = string.Empty;
		}
		else
		{
			var info = await _friendManager.GetFriendInfoAsync(e.UserId);
			senderRemark = info is null ? string.Empty : info.Remark;
		}

		var eventArgs = new AGroupMessageEventArgs()
		{
			Type = e.SubType.ToAvaQQ(),
			MessageId = e.MessageId,
			Time = e.Time,
			GroupUin = e.GroupId,
			SenderUin = isAnonymous ? e.Anonymous!.Id : e.UserId,
			AnonymousFlag = isAnonymous ? e.Anonymous!.Flag : string.Empty,
			Message = e.Message.ToAvaQQ(_logger),
			SenderNickname = isAnonymous ? e.Anonymous!.Name : e.Sender!.Nickname,
			SenderGroupNickname = isAnonymous ? string.Empty : e.Sender!.Card,
			SenderRemark = senderRemark,
			SenderLevel = isAnonymous ? 0 : int.Parse(e.Sender!.Level),
			SenderRole = isAnonymous ? GroupRoleType.Member : e.Sender!.Role.ToAvaQQ(),
			SpecificTitle = isAnonymous ? string.Empty : e.Sender!.Title,
		};

		OnGroupMessage.Invoke(this, eventArgs);
	}
}
