using AvaQQ.Core.Entities;

namespace AvaQQ.Core.Contexts;

internal partial class UserContext
{
	private void InitializeInfo()
	{
	}

	private void FinalizeInfo()
	{
	}

	private readonly ICache<ulong, UserInfo> _userCache;
}
