namespace AvaQQ.SDK;

public interface IAdapterProvider : IDisposable
{
	IAdapter? Adapter { get; set; }

	IAdapter EnsuredAdapter { get; }
}
