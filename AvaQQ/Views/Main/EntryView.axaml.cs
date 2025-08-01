using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using AvaQQ.Exceptions;
using AvaQQ.ViewModels.Main;
using Microsoft.Extensions.DependencyInjection;

namespace AvaQQ.Views.Main;

[PseudoClasses(":selected")]
public partial class EntryView : UserControl
{
	public EntryViewModel ViewModel
	{
		get => (EntryViewModel)(DataContext ?? throw new NotInitializedException(nameof(ViewModel)));
		set => DataContext = value;
	}

	public EntryView(EntryViewModel viewModel)
	{
		ViewModel = viewModel;
		InitializeComponent();

		DesignerHelper.ShowGridBackground(this);
	}

	public EntryView() : this(DesignerHelper.Services.GetRequiredService<EntryViewModel>())
	{
	}

	private DateTime _lastLeftButtonPressTime = DateTime.MinValue;

	private static readonly TimeSpan _doubleClickTimeSpan = TimeSpan.FromMilliseconds(500);

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);

		if (e.Properties.IsLeftButtonPressed)
		{
			Select();
		}

		if (e.Properties.IsLeftButtonPressed)
		{
			var now = DateTime.Now;
			if (now - _lastLeftButtonPressTime <= _doubleClickTimeSpan)
			{
				DoubleClicked?.Invoke(this, EventArgs.Empty);
				_lastLeftButtonPressTime = DateTime.MinValue;
			}
			else
			{
				_lastLeftButtonPressTime = now;
			}
		}
	}

	public event EventHandler? Selected;

	public void Select()
	{
		PseudoClasses.Set(":selected", true);
		Selected?.Invoke(this, EventArgs.Empty);
	}

	public void Deselect()
	{
		PseudoClasses.Set(":selected", false);
	}

	public event EventHandler? DoubleClicked;
}
