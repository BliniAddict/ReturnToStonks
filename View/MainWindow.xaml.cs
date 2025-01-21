using ReturnToStonks.View.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReturnToStonks
{
  public partial class MainWindow : Window, IView
  {
    private MainViewModel _viewModel;
    private MessageService _messageService;
    public MainWindow()
    {
      InitializeComponent();

      _messageService = new MessageService();
      _viewModel = new MainViewModel(this, new Model(), _messageService);
      _messageService.RegisterView(this);
      DataContext = _viewModel;
    }

    public void CloseWindow() => Close();

    public void ShowMessage(string message)
    {
      NotificationPopup notification = new()
      {
        Width = 300,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Bottom,
        Margin = new Thickness(0, 0, 0, 50)
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

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (sender is DataGrid dataGrid && dataGrid.SelectedItem is Transaction transaction)
      {
        if (transaction.Date <= new DateTime(DateTime.Today.Year, DateTime.Today.Month + 1, 1))
          _viewModel.OpenTransactionWindow(transaction);
      }
    }
  }
}