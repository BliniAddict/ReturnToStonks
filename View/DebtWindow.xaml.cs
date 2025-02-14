using System.Windows;

namespace ReturnToStonks
{
  public partial class DebtWindow : Window, IView
  {
    private DebtViewModel _viewModel;
    public readonly MessageService _messageService;

    public DebtWindow(IModel model, MessageService messageService, Debt debt)
    {
      InitializeComponent();

      _messageService = messageService;
      _viewModel = new DebtViewModel(this, model, _messageService, debt);
      _messageService.RegisterView(this);
      DataContext = _viewModel;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {

    }
  }
}
