using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using System;
using System.Linq;

namespace AvaQQ.Views.MainPanels;

[PseudoClasses(":selected")]
public partial class CategoryButton : Button
{
	public static readonly RoutedEvent<RoutedEventArgs> SelectedEvent =
		RoutedEvent.Register<CategoryButton, RoutedEventArgs>(
			nameof(Selected),
			RoutingStrategies.Bubble);

	public event EventHandler<RoutedEventArgs>? Selected
	{
		add => AddHandler(SelectedEvent, value);
		remove => RemoveHandler(SelectedEvent, value);
	}
	
	public static readonly RoutedEvent<RoutedEventArgs> DeselectedEvent =
		RoutedEvent.Register<CategoryButton, RoutedEventArgs>(
			nameof(Deselected),
			RoutingStrategies.Bubble);

	public event EventHandler<RoutedEventArgs>? Deselected
	{
		add => AddHandler(DeselectedEvent, value);
		remove => RemoveHandler(DeselectedEvent, value);
	}

	public CategoryButton() : base()
	{
		InitializeComponent();

		Click += CategoryButton_Click;
	}

	private void CategoryButton_Click(object? sender, RoutedEventArgs e)
	{
		IsSelected = true;
	}

	public int GroupId { get; set; }

	private bool _isSelected;

	public bool IsSelected
	{
		get => _isSelected;
		set
		{
			if (_isSelected == value)
			{
				return;
			}

			_isSelected = value;

			if (_isSelected == false)
			{
				PseudoClasses.Remove(":selected");
				RaiseEvent(new(DeselectedEvent));
				return;
			}

			PseudoClasses.Add(":selected");
			DeselectOthers();
			RaiseEvent(new(SelectedEvent));
		}
	}

	private void DeselectOthers()
	{
		if (Parent is Visual parent)
		{
			parent.GetVisualChildren()
				.OfType<CategoryButton>()
				.Where(x => x.GroupId == GroupId && x != this)
				.ToList()
				.ForEach(x => x.IsSelected = false);
		}
	}
}
