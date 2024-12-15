using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AvaQQ.Core.Views.MainPanels;

/// <summary>
/// ���ఴť
/// </summary>
[PseudoClasses(":selected")]
public partial class CategoryButton : Button
{
	/// <summary>
	/// ѡ���¼�
	/// </summary>
	public static readonly RoutedEvent<RoutedEventArgs> SelectedEvent =
		RoutedEvent.Register<CategoryButton, RoutedEventArgs>(
			nameof(Selected),
			RoutingStrategies.Bubble);

	/// <summary>
	/// ��ѡ��ʱ����
	/// </summary>
	public event EventHandler<RoutedEventArgs>? Selected
	{
		add => AddHandler(SelectedEvent, value);
		remove => RemoveHandler(SelectedEvent, value);
	}

	/// <summary>
	/// ȡ��ѡ���¼�
	/// </summary>
	public static readonly RoutedEvent<RoutedEventArgs> DeselectedEvent =
		RoutedEvent.Register<CategoryButton, RoutedEventArgs>(
			nameof(Deselected),
			RoutingStrategies.Bubble);

	/// <summary>
	/// ��ȡ��ѡ��ʱ����
	/// </summary>
	public event EventHandler<RoutedEventArgs>? Deselected
	{
		add => AddHandler(DeselectedEvent, value);
		remove => RemoveHandler(DeselectedEvent, value);
	}

	/// <summary>
	/// �������ఴť
	/// </summary>
	public CategoryButton() : base()
	{
		InitializeComponent();

		Click += CategoryButton_Click;
	}

	private void CategoryButton_Click(object? sender, RoutedEventArgs e)
	{
		IsSelected = true;
	}

	/// <summary>
	/// ID
	/// </summary>
	public int Id { get; set; }

	private bool _isSelected;

	/// <summary>
	/// �Ƿ�ѡ��
	/// </summary>
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
				.Where(x => x.Id == Id && x != this)
				.ToList()
				.ForEach(x => x.IsSelected = false);
		}
	}
}
