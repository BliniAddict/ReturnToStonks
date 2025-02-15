using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ReturnToStonks
{
  public class TransactionViewModel : ViewModelBase
  {
    private Transaction? _oldTransaction;

    public TransactionViewModel(IView view, IModel model, MessageService messageService, Transaction? transaction)
    {
      base.view = view;
      base.model = model;
      base.messageService = messageService;

      InitTransactionWindow(transaction);

      SaveTransactionCommand = new RelayCommand(SaveTransaction);
      SaveCategoryCommand = new RelayCommand(SaveCategory);
      ChangeCategoryCommand = new RelayCommand<Category>(InitCategoryPopup);
      DeleteTransactionCommand = new RelayCommand(DeleteTransaction);
      DeleteCategoryCommand = new RelayCommand(DeleteCategory);
    }

    public ICommand SaveTransactionCommand { get; }
    public ICommand DeleteTransactionCommand { get; }

    public ICommand SaveCategoryCommand { get; }
    public ICommand ChangeCategoryCommand { get; }
    public ICommand DeleteCategoryCommand { get; }

    #region Properties
    private Transaction _selectedTransaction;
    public Transaction SelectedTransaction
    {
      get
      {
        Transaction? tempTransaction = _oldTransaction == null ? null : new Transaction(_oldTransaction)
        { Amount = Math.Abs(_oldTransaction.Amount) };
        IsDeleteTransactionButtonEnabled = Utilities.ArePropertiesEqual(_selectedTransaction, tempTransaction);

        return _selectedTransaction;
      }
      set
      {
        _selectedTransaction = value;
        OnPropertyChanged();
      }
    }

    private bool _isIncome = false;
    public bool IsIncome
    {
      get
      {
        IsDeleteTransactionButtonEnabled = _isIncome == (_oldTransaction?.Amount < 0);
        return _isIncome;
      }
      set
      {
        _isIncome = value;
        OnPropertyChanged();
      }
    }

    private bool _isDeleteTransactionButtonEnabled;
    public bool IsDeleteTransactionButtonEnabled
    {
      get => _isDeleteTransactionButtonEnabled;
      set
      {
        _isDeleteTransactionButtonEnabled = value;
        OnPropertyChanged();
      }
    }

    private bool _isDeleteCategoryButtonEnabled;
    public bool IsDeleteCategoryButtonEnabled
    {
      get => _isDeleteCategoryButtonEnabled;
      set
      {
        _isDeleteCategoryButtonEnabled = value;
        OnPropertyChanged();
      }
    }
    #endregion

    #region Methods
    private void InitTransactionWindow(Transaction? transaction = null)
    {
      if (transaction == null)
        transaction = new Transaction(string.Empty, null, 0, DateTime.Now, false, new("month", 1));
      else
      {
        transaction.Recurrence ??= new("month", 1);
        _oldTransaction = new Transaction(transaction);

        if (transaction.Amount < 0)
          transaction.Amount *= -1;
        else
          IsIncome = true;
      }

      SelectedTransaction = transaction;
      GetCategories();
    }

    private void SaveTransaction()
    {
      SelectedTransaction.Category = SelectedCategory;

      if (!IsIncome)
        SelectedTransaction.Amount *= -1;

      string message = model.SaveTransaction(SelectedTransaction, _oldTransaction);
      messageService.ShowMessage(message, true);
      view.CloseWindow();
    }
    private void DeleteTransaction()
    {
      string? additionaMessage = SelectedTransaction.IsRecurring ? "Recurring Transactions will also be affected." : null;
      if (HasUserConfirmed("delete", SelectedTransaction, additionaMessage))
      {
        if (!IsIncome)
          SelectedTransaction.Amount = SelectedTransaction.Amount * -1;

        if (!SelectedTransaction.IsRecurring)
          SelectedTransaction.Recurrence = null;

        string message = model.DeleteTransaction(SelectedTransaction);
        messageService.ShowMessage(message, true);

        view.CloseWindow();
      }
    }

    public override void GetCategories()
    {
      base.GetCategories();
      if (SelectedTransaction.Category != null)
        SelectedCategory = Categories.FirstOrDefault(name => SelectedTransaction.Category.Name == name.Name);
    }
    public override void CheckIfCategoryChanged()
    {
      Transaction? tempTransaction = _oldTransaction == null ? null : new Transaction(_oldTransaction)
      {
        Amount = Math.Abs(_oldTransaction.Amount),
        Category = _selectedCategory
      };

      IsDeleteTransactionButtonEnabled = Utilities.ArePropertiesEqual(_selectedTransaction, tempTransaction);
      IsDeleteCategoryButtonEnabled = Utilities.ArePropertiesEqual(_selectedCategory, OldCategory);
    }
    #endregion
  }
}