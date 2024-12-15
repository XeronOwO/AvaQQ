using Avalonia.Controls;
using AvaQQ.Core.Resources;

namespace AvaQQ.Core.MainPanels;

internal class RecentCategorySelection : ICategorySelection
{
	public UserControl? View => null;

	public void OnDeselected()
	{

	}

	public void OnSelected()
	{

	}

	public override string ToString()
	{
		return SR.TextRecent;
	}

	#region Dispose

	private bool disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
			}

			disposedValue = true;
		}
	}

	~RecentCategorySelection()
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
