using ReturnToStonks.View.Controls;
using System.Windows;
using System.Windows.Controls;

namespace ReturnToStonks
{
  public partial class TransactionWindow : Window, IView
  {
    private TransactionViewModel _viewModel;
    private readonly MessageService _messageService;

    public TransactionWindow(IModel model, MessageService messageService, Transaction transaction)
    {
      InitializeComponent();

      _messageService = messageService;
      _viewModel = new TransactionViewModel(this, model, _messageService, transaction);
      _messageService.RegisterView(this);
      DataContext = _viewModel;
    }

    public void CloseWindow()
    {
      _messageService.UnregisterView(this);
      Close();
    }

    public void ShowMessage(string message)
    {
      NotificationPopup notification = new()
      {
        Width = 300,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Bottom,
        Margin = new Thickness(0, 0, 0, 20)
      };

      if (Content is Grid grid)
      {
        grid.Children.Add(notification);
        grid.Children[^1].SetValue(Grid.ColumnSpanProperty, 10);
        grid.Children[^1].SetValue(Grid.RowSpanProperty, 10);

        notification.ShowMessage(message);

        // Optional: Nach einer kurzen Zeit entfernen
        Task.Delay(3000).ContinueWith(_ =>
        {
          grid.Dispatcher.Invoke(() => { grid.Children.Remove(notification); });
        });
      }
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
          CloseWindow();
      }
    }
  }
}