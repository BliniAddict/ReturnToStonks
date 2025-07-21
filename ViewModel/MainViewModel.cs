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
using System.Transactions;
using System.Windows.Input;

namespace ReturnToStonks
{
    public class MainViewModel : ViewModelBase
    {
        private readonly MessageService _messageService;
        private IView _view;
        private IModel _model;

        public MainViewModel(IView view, IModel model, MessageService messageService)
        {
            _view = view;
            _model = model;
            _messageService = messageService;

            GetDebts();
            GetTransactions();

            AddTransactionCommand = new RelayCommand<Transaction?>(OpenTransactionWindow);
            AddDebtCommand = new RelayCommand<Debt?>(OpenDebtWindow);
        }

        public ICommand AddTransactionCommand { get; }
        public ICommand AddDebtCommand { get; }

        #region Properties
        #region Transactions
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

        private bool _showOneTime = true;
        public bool ShowOneTime
        {
            get => _showOneTime;
            set
            {
                _showOneTime = value;
                OnPropertyChanged();

                if (!value && !ShowRecurring)
                    ShowRecurring = true;
                GetTransactions();
            }
        }

        private bool _showRecurring = true;
        public bool ShowRecurring
        {
            get => _showRecurring;
            set
            {
                _showRecurring = value;
                OnPropertyChanged();

                if (!value && !ShowOneTime)
                    ShowOneTime = true;
                GetTransactions();
            }
        }
        #endregion

        #region Debts
        private ObservableCollection<Person> _people { get; set; } = new ObservableCollection<Person>();
        public ObservableCollection<Person> People
        {
            get => _people;
            set
            {
                _people = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Debt> _debtPendingToOthers { get; set; } = new ObservableCollection<Debt>();
        public ObservableCollection<Debt> DebtPendingToOthers
        {
            get => _debtPendingToOthers;
            set
            {
                _debtPendingToOthers = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Debt> _debtPendingToMe { get; set; } = new ObservableCollection<Debt>();
        public ObservableCollection<Debt> DebtPendingToMe
        {
            get => _debtPendingToMe;
            set
            {
                _debtPendingToMe = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #endregion

        #region Methods
        public void OpenTransactionWindow(Transaction? transaction = null)
        {
            TransactionWindow newTransaction = new(_model, _messageService, transaction);
            newTransaction.ShowDialog();

            GetTransactions();
        }
        public void OpenDebtWindow(Debt? debt = null)
        {
            DebtWindow newDebt = new(_model, _messageService, debt);
            newDebt.ShowDialog();

            GetDebts();
        }

        private void GetTransactions()
        {
            Incomes.Clear();
            Expenses.Clear();

            List<Transaction> transactions = _model.GetTransactions().ToList();
            transactions = SetFutureTransactions(transactions).Where(date => date.Date >= Utilities.ThisMonth.AddMonths(-1)).ToList();
            foreach (Transaction transaction in transactions.Where(o => o.IsRecurring != ShowOneTime || o.IsRecurring == ShowRecurring))
            {
                if (transaction.Amount > 0)
                    Incomes.Add(transaction);
                else
                    Expenses.Add(transaction);
            }
            IncomesSum = Incomes.Where(paid => paid.Date <= DateTime.Today).Sum(amount => amount.Amount);
            ExpensesSum = Expenses.Where(paid => paid.Date <= DateTime.Today).Sum(amount => amount.Amount);
        }
        private List<Transaction> SetFutureTransactions(List<Transaction> transactions)
        {
            ObservableCollection<Transaction> newTransactions = new(transactions);
            DateTime today = DateTime.Today;

            foreach (Transaction transaction in transactions.Where(recurring => recurring.IsRecurring))
            {
                Transaction? futureTr = new(transaction);
                futureTr.Date = Utilities.CalculateNextDueDate(futureTr.Date, futureTr.Recurrence);
                futureTr.IsPayed = futureTr.Date <= today;

                while (futureTr.Date < Utilities.In2Months)
                {
                    if (!newTransactions.Any(a => Utilities.ArePropertiesEqual(a, futureTr)))
                    {
                        newTransactions.Add(futureTr);
                        _model.SaveTransaction(futureTr);

                        if (futureTr.IsPayed != null && (bool)futureTr.IsPayed)
                            _model.SaveTransaction(futureTr);
                    }
                    futureTr = new(futureTr);
                    futureTr.Date = Utilities.CalculateNextDueDate(futureTr.Date, futureTr.Recurrence);
                    futureTr.IsPayed = futureTr.Date <= today;
                }
            }
            return new List<Transaction>(newTransactions.OrderBy(a => a.Amount).OrderBy(d => d.Date));
        }

        private void GetDebts()
        {
            DebtPendingToOthers.Clear();
            DebtPendingToMe.Clear();
            People.Clear();

            List<Debt> debts = _model.GetDebts().ToList();

            foreach (Debt debt in debts)
            {
                if (debt.Amount < 0)
                    DebtPendingToOthers.Add(debt);
                else
                    DebtPendingToMe.Add(debt);
            }
            foreach (Person person in _model.GetPersons())
                People.Add(person);
        }
        #endregion
    }
}