using AvaQQ.Adapters.Lagrange.Utils;
using AvaQQ.SDK;
using AvaQQ.SDK.Entities;
using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Event.EventArg;
using Microsoft.Extensions.Logging;
using LagrangeLogLevel = Lagrange.Core.Event.EventArg.LogLevel;
using MicrosoftLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AvaQQ.Adapters.Lagrange;

internal class Adapter : IAdapter
{
	private readonly ILogger<Adapter> _logger;

	private readonly ILoggerProvider _loggerProvider;

	private readonly BotContext _context;

	public Adapter(ILogger<Adapter> logger, ILoggerProvider loggerProvider, BotContext context)
	{
		_logger = logger;
		_loggerProvider = loggerProvider;
		_context = context;

		_context.Invoker.OnBotLogEvent += OnBotLog;
		_context.Invoker.OnGroupMessageReceived += OnGroupMessageReceived;
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			_context.Invoker.OnBotLogEvent -= OnBotLog;
			_context.Invoker.OnGroupMessageReceived -= OnGroupMessageReceived;

			if (disposing)
			{
				_context.Dispose();
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

	private void OnBotLog(BotContext context, BotLogEvent e)
	{
		_loggerProvider.CreateLogger(e.Tag).Log(e.Level switch
		{
			LagrangeLogLevel.Debug => MicrosoftLogLevel.Trace,
			LagrangeLogLevel.Verbose => MicrosoftLogLevel.Information,
			LagrangeLogLevel.Information => MicrosoftLogLevel.Information,
			LagrangeLogLevel.Warning => MicrosoftLogLevel.Warning,
			LagrangeLogLevel.Fatal => MicrosoftLogLevel.Error,
			_ => MicrosoftLogLevel.Error
		}, "{}", e.ToString());
	}

	public Task<bool> TryLoginByEasyAsync(CancellationToken token = default)
	{
		return Task.Run(() => // 防止阻塞 UI 线程
		{
			var keystore = _context.UpdateKeystore();
			if (keystore.Session.TempPassword == null) return Task.FromResult(false);
			if (keystore.Session.TempPassword.Length == 0) return Task.FromResult(false);
			return _context.LoginByEasy(token);
		}, token);
	}

	public async Task<bool> TryLoginByQrCodeAsync(Action<byte[]> qrCodeCallback, CancellationToken token = default)
	{
		var fetchQrCodeTask = Task.Run(() => // 防止阻塞 UI 线程
		{
			return _context.FetchQrCode().WaitAsync(token);
		}, token);

		(string Url, byte[] QrCode)? qrCode = await fetchQrCodeTask;
		if (!qrCode.HasValue) return false;

		qrCodeCallback(qrCode.Value.QrCode);

		return await (Task<bool>)_context.LoginByQrCode(token);
	}

	public ulong Uin => _context.BotUin;

	public async Task<AdaptedUserInfo?> GetUserAsync(ulong uin, CancellationToken token = default)
	{
		try
		{
			if (await _context.FetchUserInfo((uint)uin).WithCancellationToken(token) is not { } user)
			{
				return null;
			}
			return new AdaptedUserInfo(user.Uin, user.Nickname, string.IsNullOrEmpty(user.Remark) ? null : user.Remark);
		}
		catch (OperationCanceledException)
		{
			_logger.LogWarning("Fetching user info of {Uin} was cancelled", uin);
			return null;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get user info of {Uin}", uin);
			return null;
		}
	}

	public async Task<AdaptedUserInfo[]> GetAllFriendsAsync(CancellationToken token = default)
	{
		try
		{
			var result = new List<AdaptedUserInfo>();
			foreach (var friend in await _context.FetchFriends().WithCancellationToken(token))
			{
				result.Add(new(friend.Uin, friend.Nickname, string.IsNullOrEmpty(friend.Remarks) ? null : friend.Remarks));
			}
			return [.. result];
		}
		catch (OperationCanceledException)
		{
			_logger.LogWarning("Fetching all friends was cancelled");
			return [];
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get all friends");
			return [];
		}
	}

	public async Task<AdaptedGroupInfo[]> GetAllJoinedGroupsAsync(CancellationToken token = default)
	{
		try
		{
			var result = new List<AdaptedGroupInfo>();
			foreach (var group in await _context.FetchGroups(true).WithCancellationToken(token))
			{
				result.Add(new(group.GroupUin, group.GroupName, string.IsNullOrEmpty(group.GroupRemark) ? null : group.GroupRemark));
			}
			return [.. result];
		}
		catch (OperationCanceledException)
		{
			_logger.LogWarning("Fetching all joined groups was cancelled");
			return [];
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get all joined groups");
			return [];
		}
	}

	public async Task<AdaptedGroupInfo?> GetJoinedGroupAsync(ulong uin, CancellationToken token = default)
	{
		try
		{
			(var code, var message, var info) = await _context.FetchGroupInfo(uin).WithCancellationToken(token);
			if (code != 0)
			{
				_logger.LogError("Failed to get group: code={Code}, message={Message}", code, message);
				return null;
			}
			return new AdaptedGroupInfo(info.Uin, info.Name, null);
		}
		catch (OperationCanceledException)
		{
			_logger.LogWarning("Fetching group info of {Uin} was cancelled", uin);
			return null;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to get group info of {Uin}", uin);
			return null;
		}
	}

	private void OnGroupMessageReceived(BotContext context, GroupMessageEvent e)
	{
		
	}
}
