using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ReturnToStonks
{
  public partial class TransactionWindow : Window, IView
  {
    private TransactionViewModel _viewModel;
    public TransactionWindow(IModel model, Transaction transaction)
    {
      InitializeComponent();

      _viewModel = new TransactionViewModel(this, model, transaction);
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
          Close();
      }
    }
  }
}
