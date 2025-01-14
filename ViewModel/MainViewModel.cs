using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

      GetTransactions();

      AddTransactionCommand = new RelayCommand<Transaction?>(OpenTransactionWindow);
    }

    public ICommand AddTransactionCommand { get; }

    public ObservableCollection<Transaction> Incomes { get; set; } = new ObservableCollection<Transaction>();
    public ObservableCollection<Transaction> Expenses { get; set; } = new ObservableCollection<Transaction>();

    private void GetTransactions()
    {
      Incomes.Clear();
      Expenses.Clear();

      List<Transaction> transactions = _model.GetTransactions();
      foreach (Transaction transaction in transactions)
      {
        if (transaction.Amount > 0)
          Incomes.Add(transaction);
        else
          Expenses.Add(transaction);
      }
    }

    public void OpenTransactionWindow(Transaction? transaction = null)
    {
      TransactionWindow newTransaction = new(_model, transaction);
      newTransaction.ShowDialog();

      GetTransactions();
    }
  }
}
