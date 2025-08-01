using Avalonia.Controls;
using AvaQQ.SDK;

namespace AvaQQ.Adapters.Lagrange;

internal class AdapterViewImpl : IAdapterView
{
	private static UserControl NewView => new AdapterView();

	private UserControl? _view;

	public UserControl View
	{
		get
		{
			if (Design.IsDesignMode)
			{
				return NewView;
			}
			return _view ??= NewView;
		}
	}

	public string Id => nameof(Lagrange);

	public void OnSelected()
	{
	}

	public void OnDeselected()
	{
	}

	public void OnRelease()
		=> _view = null;

	public override string ToString()
		=> SR.TextLagrangeAdapter;
}
