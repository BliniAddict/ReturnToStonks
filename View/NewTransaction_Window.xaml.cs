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

namespace ReturnToStonks.View
{
  public partial class NewTransaction_Window : Window, IView
  {
    public NewTransaction_Window(IModel model, Transaction transaction)
    {
      InitializeComponent();

      TransactionViewModel viewModel = new TransactionViewModel(this, model, transaction);
      DataContext = viewModel;
    }

  }
}
