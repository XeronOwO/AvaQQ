using AvaQQ.Core.Adapters;
using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Event.EventArg;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LagrangeLogLevel = Lagrange.Core.Event.EventArg.LogLevel;
using MicrosoftLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AvaQQ.Adapters.Lagrange;

internal class Adapter : IAdapter
{
	private readonly ILogger<Adapter> _logger;

	private readonly ILogger<BotContext> _botLogger;

	private readonly BotContext _context;

	public Adapter(IServiceProvider serviceProvider, BotContext context)
	{
		_logger = serviceProvider.GetRequiredService<ILogger<Adapter>>();
		_botLogger = serviceProvider.GetRequiredService<ILogger<BotContext>>();

		_context = context;
		_context.Invoker.OnBotLogEvent += OnBotLog;
	}

	private void OnBotLog(BotContext context, BotLogEvent e)
	{
		_botLogger.Log(e.Level switch
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
		var keystore = _context.UpdateKeystore();
		if (keystore.Session.TempPassword == null) return Task.FromResult(false);
		if (keystore.Session.TempPassword.Length == 0) return Task.FromResult(false);
		return _context.LoginByEasy(token);
	}

	public async Task<bool> TryLoginByQrCodeAsync(Action<byte[]> qrCodeCallback, CancellationToken token = default)
	{
		(string Url, byte[] QrCode)? qrCode = await _context.FetchQrCode().WaitAsync(token);
		if (!qrCode.HasValue) return false;

		qrCodeCallback(qrCode.Value.QrCode);

		return await (Task<bool>)_context.LoginByQrCode(token);
	}

	public ulong Uin => _context.BotUin;

	public async Task<AdaptedUserInfo?> GetUserAsync(ulong uin, CancellationToken token = default)
	{
		if (await _context.FetchUserInfo((uint)uin) is not { } user)
		{
			return null;
		}
		return new AdaptedUserInfo(user.Uin, user.Nickname, string.IsNullOrEmpty(user.Remark) ? null : user.Remark);
	}

	public async Task<AdaptedUserInfo[]> GetAllFriendsAsync(CancellationToken token = default)
	{
		var result = new List<AdaptedUserInfo>();
		foreach (var friend in await _context.FetchFriends())
		{
			result.Add(new(friend.Uin, friend.Nickname, string.IsNullOrEmpty(friend.Remarks) ? null : friend.Remarks));
		}
		return [.. result];
	}

	public async Task<AdaptedGroupInfo[]> GetAllJoinedGroupsAsync(CancellationToken token = default)
	{
		var result = new List<AdaptedGroupInfo>();
		foreach (var group in await _context.FetchGroups(true))
		{
			result.Add(new(group.GroupUin, group.GroupName, string.IsNullOrEmpty(group.GroupRemark) ? null : group.GroupRemark));
		}
		return [.. result];
	}

	public async Task<AdaptedGroupInfo?> GetJoinedGroupAsync(ulong uin, CancellationToken token = default)
	{
		(var code, var message, var info) = await _context.FetchGroupInfo(uin);
		if (code != 0)
		{
			_logger.LogError("Failed to get group: code={Code}, message={Message}", code, message);
			return null;
		}
		return new AdaptedGroupInfo(info.Uin, info.Name, null);
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
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
}
