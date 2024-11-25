using Avalonia.Controls;
using AvaQQ.Resources;
using AvaQQ.SDK.MainPanels;
using System;

namespace AvaQQ.MainPanels;

internal class RecentCategorySelection : ICategorySelection
{
	public UserControl? UserControl => null;

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
