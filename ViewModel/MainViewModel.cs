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

    #region Properties
    private ObservableCollection<Transaction> _incomes { get; set; } = new ObservableCollection<Transaction>();
    public ObservableCollection<Transaction> Incomes
    {
      get => _incomes;
      set
      {
        _incomes = value;
        OnPropertyChanged();
      }
    }

    private ObservableCollection<Transaction> _expenses = new ObservableCollection<Transaction>();
    public ObservableCollection<Transaction> Expenses
    {
      get => _expenses;
      set
      {
        _expenses = value;
        OnPropertyChanged();
      }
    }

    private double _incomesSum;
    public double IncomesSum
    {
      get => _incomesSum;
      set
      {
        _incomesSum = value;
        OnPropertyChanged();
      }
    }

    private double _expensesSum;
    public double ExpensesSum
    {
      get => _expensesSum; set
      {
        _expensesSum = value;
        OnPropertyChanged();
      }
    }
    #endregion

    #region Methods
    private void GetTransactions()
    {
      Incomes.Clear();
      Expenses.Clear();

      List<Transaction> transactions = _model.GetTransactions(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).ToList();
      foreach (Transaction transaction in transactions)
      {
        if (transaction.Amount > 0)
          Incomes.Add(transaction);
        else
          Expenses.Add(transaction);
      }
      IncomesSum = Incomes.Sum(amount => amount.Amount);
      ExpensesSum = Expenses.Sum(amount => amount.Amount);

      Incomes = SetFutureTransactions(Incomes);
      Expenses = SetFutureTransactions(Expenses);
    }

    private ObservableCollection<Transaction> SetFutureTransactions(ObservableCollection<Transaction> transactions)
    {
      ObservableCollection<Transaction> newTransactions = new ObservableCollection<Transaction>(transactions);
      DateTime today = DateTime.Today;

      foreach (Transaction transaction in transactions.Where(recurring => recurring.IsRecurring))
      {
        Transaction? futureTr = new(transaction);
        futureTr.Date = CalculateNextDueDate(futureTr.Date, futureTr.Recurrence);
        while (futureTr.Date < new DateTime(today.Year, today.Month + 2, 1))
        {
          if (!newTransactions.Any(a => ArePropertiesEqual(a, futureTr)))
          {
            newTransactions.Add(futureTr);

            if (futureTr.Date <= today)
              _model.SaveTransaction(futureTr);
          }
          futureTr = new(futureTr);
          futureTr.Date = CalculateNextDueDate(futureTr.Date, futureTr.Recurrence);
        }
      }
      return new ObservableCollection<Transaction>(newTransactions.OrderBy(a => a.Amount).OrderBy(d => d.Date));
    }

    public void OpenTransactionWindow(Transaction? transaction = null)
    {
      TransactionWindow newTransaction = new(_model, transaction);
      newTransaction.ShowDialog();

      GetTransactions();
    }
    #endregion
  }
}