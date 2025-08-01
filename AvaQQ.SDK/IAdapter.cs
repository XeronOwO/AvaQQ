using AvaQQ.SDK.Entities;

namespace AvaQQ.SDK;

public interface IAdapter : IDisposable
{
	ulong Uin { get; }

	Task<AdaptedUserInfo?> GetUserAsync(ulong uin, CancellationToken token = default);
}
