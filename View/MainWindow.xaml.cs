using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReturnToStonks
{
  public partial class MainWindow : Window, IView
  {
    private MainViewModel _viewModel;
    public MainWindow()
    {
      InitializeComponent();

      _viewModel = new MainViewModel(this, new Model());
      DataContext = _viewModel;
    }

    public void CloseWindow() => Close();

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (sender is DataGrid dataGrid && dataGrid.SelectedItem is Transaction transaction)
      {
        if (transaction.Date <= DateTime.Today)
          _viewModel.OpenTransactionWindow(transaction);
      }
    }
  }
}