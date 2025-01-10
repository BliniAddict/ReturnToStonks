using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReturnToStonks
{
  public class MainViewModel : ViewModelBase
  {
    private IView _view;
    private IModel _model;

    public MainViewModel(IView view, IModel model)
    {
      _view = view;
      _model = model;

      AddTransactionCommand = new RelayCommand(OpenTransactionWindow);
    }

    public ICommand AddTransactionCommand { get; }

    private void OpenTransactionWindow()
    {
      TransactionWindow newTransaction = new(_model, new Transaction(0.00, null, false, DateTime.Now, ""));
      newTransaction.ShowDialog();
    }
  }
}
