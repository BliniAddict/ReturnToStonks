using System.Windows;
using System.Windows.Controls;

namespace ReturnToStonks
{
  public partial class TransactionWindow : Window, IView
  {
    private TransactionViewModel _viewModel;
    public readonly MessageService _messageService;

    public TransactionWindow(IModel model, MessageService messageService, Transaction transaction)
    {
      InitializeComponent();

      _messageService = messageService;
      _viewModel = new TransactionViewModel(this, model, _messageService, transaction);
      _messageService.RegisterView(this);
      DataContext = _viewModel;
    }

    public void OpenCategoryPopup()
    {
      NewCategoryPopup.IsOpen = true;
    }
    public void CloseCategoryPopup()
    {
      NewCategoryPopup.IsOpen = false;
      _viewModel.GetCategories();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      if (sender is Button btn)
      {
        if (btn.Name == "CancelNewCategory")
          CloseCategoryPopup();
        else
          ((IView)this).CloseWindow(this);
      }
    }
  }
}