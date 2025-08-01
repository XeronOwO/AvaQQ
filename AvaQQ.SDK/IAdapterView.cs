using Avalonia.Controls;

namespace AvaQQ.SDK;

public interface IAdapterView
{
	UserControl View { get; }

	string Id { get; }

	void OnSelected() { }

	void OnDeselected() { }

	void OnRelease();
}
