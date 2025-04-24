using AvaQQ.SDK;
using Lagrange.Core;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace AvaQQ.Adapters.Lagrange;

internal static class BotContextHelper
{
	public static Task<BotContext> CreateBotContextAsync(IConfiguration configuration, CancellationToken token)
		=> Task.Run(() => BotFactory.Create(
			CreateBotConfig(configuration),
			GetOrCreateBotDevice(configuration),
			GetOrCreateKeyStore(configuration)
			), token);

	private static BotConfig CreateBotConfig(IConfiguration configuration)
		=> new()
		{
			Protocol = configuration["Account:Protocol"] switch
			{
				"Windows" => Protocols.Windows,
				"MacOs" => Protocols.MacOs,
				_ => Protocols.Linux,
			},
			AutoReconnect = configuration.GetValue("Account:AutoReconnect", true),
			UseIPv6Network = configuration.GetValue("Account:UseIPv6Network", false),
			GetOptimumServer = configuration.GetValue("Account:GetOptimumServer", true),
			AutoReLogin = configuration.GetValue("Account:AutoReLogin", true),
		};

	private static BotDeviceInfo GetOrCreateBotDevice(IConfiguration configuration)
	{
		var path = configuration.GetValue("ConfigPath:DeviceInfo", Path.Combine(Configuration.BaseDirectory, "device.json"))!;

		var device = File.Exists(path)
			? JsonSerializer.Deserialize<BotDeviceInfo>(File.ReadAllText(path)) ?? BotDeviceInfo.GenerateInfo()
			: CreateBotDevice();

		var deviceJson = JsonSerializer.Serialize(device);
		File.WriteAllText(path, deviceJson);

		return device;
	}

	private static BotDeviceInfo CreateBotDevice()
	{
		var info = BotDeviceInfo.GenerateInfo();
		info.DeviceName = $"AvaQQ-{info.DeviceName}";
		return info;
	}

	private static BotKeystore GetOrCreateKeyStore(IConfiguration configuration)
	{
		var path = configuration.GetValue("ConfigPath:Keystore", Path.Combine(Configuration.BaseDirectory, "keystore.json"))!;

		return File.Exists(path)
			? JsonSerializer.Deserialize<BotKeystore>(File.ReadAllText(path)) ?? new()
			: new();
	}
}
