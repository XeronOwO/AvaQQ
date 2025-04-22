using AvaQQ.SDK;
using Lagrange.Core;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace AvaQQ.Adapters.Lagrange;

internal static class BotContextHelper
{
	public static Task<BotContext> CreateBotContextAsync(IConfiguration configuration)
		=> Task.Run(() => BotFactory.Create(
			CreateBotConfig(configuration),
			GetOrCreateBotDevice(configuration),
			GetOrCreateKeyStore(configuration)
			));

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
		var path = configuration["ConfigPath:DeviceInfo"] ?? Path.Combine(Configuration.BaseDirectory, "device.json");

		var device = File.Exists(path)
			? JsonSerializer.Deserialize<BotDeviceInfo>(File.ReadAllText(path)) ?? BotDeviceInfo.GenerateInfo()
			: BotDeviceInfo.GenerateInfo();

		var deviceJson = JsonSerializer.Serialize(device);
		File.WriteAllText(path, deviceJson);

		return device;
	}

	private static BotKeystore GetOrCreateKeyStore(IConfiguration configuration)
	{
		var path = configuration["ConfigPath:Keystore"] ?? Path.Combine(Configuration.BaseDirectory, "keystore.json");

		return File.Exists(path)
			? JsonSerializer.Deserialize<BotKeystore>(File.ReadAllText(path)) ?? new()
			: new();
	}
}
