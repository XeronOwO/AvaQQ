﻿using AvaQQ.Core.Adapters;
using AvaQQ.Core.Caches;
using AvaQQ.SDK.Logging;
using Makabaka;
using Makabaka.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using AGroupMessageEventArgs = AvaQQ.Core.Adapters.GroupMessageEventArgs;
using MGroupMessageEventArgs = Makabaka.Events.GroupMessageEventArgs;

namespace AvaQQ.Adapters.Onebot11ForwardWebSocket;

internal class Adapter : IAdapter
{
	private readonly MakabakaApp _makabaka;

	private readonly ILogger<Adapter> _logger;

	private readonly TaskCompletionSource _connectCompletionSource = new();

	private readonly LogRecorder _logRecorder = new();

	private readonly IFriendCache _friendManager;

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
		_friendManager = serviceProvider.GetRequiredService<IFriendCache>();
	}

	#region API

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

	public async Task<IEnumerable<FriendInfo>> GetFriendListAsync()
	{
		try
		{
			var friends = (await _makabaka.BotContext.GetFriendListAsync()).Result;
			return friends.Select(f => new FriendInfo()
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

	public async Task<IEnumerable<GroupInfo>> GetGroupListAsync()
	{
		try
		{
			var groups = (await _makabaka.BotContext.GetGroupListAsync()).Result;
			return groups.Select(f => new GroupInfo()
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

	public async Task<UserInfo?> GetUserInfoAsync(ulong uin)
	{
		try
		{
			var info = (await _makabaka.BotContext.GetStrangerInfoAsync(uin)).Result;
			return new UserInfo()
			{
				Uin = info.UserId,
				Nickname = info.Nickname,
				Sex = info.Sex.ToAvaQQ(),
				Age = info.Age,
			};
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get user info of {Uin}.", uin);
			return null;
		}
	}

	public async Task<IEnumerable<GroupMemberInfo>> GetGroupMemberListAsync(ulong groupUin)
	{
		try
		{
			var members = (await _makabaka.BotContext.GetGroupMemberListAsync(groupUin)).Result;
			return members.Select(m => new GroupMemberInfo()
			{
				Uin = m.UserId,
				GroupUin = groupUin,
				Card = m.Card!,
				JoinTime = m.JoinTime,
				LastSpeakTime = m.LastSentTime,
				Level = uint.Parse(m.Level),
				Role = m.Role.ToAvaQQ(),
				Title = m.Title,
			});
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get group member infos of {GroupUin}.", groupUin);
			return [];
		}
	}

	public async Task<IEnumerable<AGroupMessageEventArgs>> GetGroupMessageHistoryAsync(ulong groupUin, ulong messageId, uint count)
	{
		try
		{
			var info = (await _makabaka.BotContext.GetGroupMessageHistoryAsync(groupUin, (long)messageId, count)).Result;
			return info.Messages.Select(m => new AGroupMessageEventArgs()
			{
				Type = m.SubType.ToAvaQQ(),
				MessageId = (ulong)m.MessageId,
				Time = m.Time,
				GroupUin = m.GroupId,
				SenderUin = m.UserId,
				Message = m.Message.ToAvaQQ(_logger),
				SenderNickname = m.Sender!.Nickname,
				SenderGroupNickname = m.Sender!.Card,
				SenderRemark = string.Empty,
				SenderLevel = int.Parse(m.Sender!.Level),
				SenderRole = m.Sender!.Role.ToAvaQQ(),
				SpecificTitle = m.Sender!.Title,
			});
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to fetch group history messages of {GroupUin}.", groupUin);
			return [];
		}
	}

	#endregion

	#region 事件

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
			MessageId = (ulong)e.MessageId,
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

	#endregion

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_makabaka.StopAsync().Wait();
				_makabaka.Dispose();
			}

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
