
namespace AvaQQ.SDK;

public interface IAdapterViewProvider
{
	Dictionary<string, IAdapterView> Views { get; }

	void Register(IAdapterView view);

	void Release();
}
