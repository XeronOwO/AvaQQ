using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace AvaQQ.Views.Main;

public partial class EntryListView : UserControl
{
	private readonly IServiceProvider _serviceProvider;

	public EntryListView(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;

		InitializeComponent();
	}

	public EntryListView() : this(DesignerHelper.Services.GetRequiredService<IServiceProvider>())
	{
	}

	private readonly ObservableCollection<EntryView> _entryViews = [];

	public ObservableCollection<EntryView> EntryViews => _entryViews;

	protected override void OnInitialized()
	{
		base.OnInitialized();


	}
}
