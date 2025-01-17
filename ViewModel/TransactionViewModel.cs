using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ReturnToStonks
{
  public class TransactionViewModel : ViewModelBase
  {
    private readonly IView _view;
    private readonly IModel _model;
    private Transaction? _oldTransaction;
    private Category? _oldCategory;

    public TransactionViewModel(IView view, IModel model, Transaction? transaction)
    {
      _view = view;
      _model = model;

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
        IsDeleteTransactionButtonEnabled = ArePropertiesEqual(_selectedTransaction, _oldTransaction);
        return _selectedTransaction;
      }
      set
      {
        _selectedTransaction = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<Category> _categories;
    public ObservableCollection<Category> Categories
    {
      get => _categories;
      set
      {
        _categories = value;
        OnPropertyChanged();
      }
    }

    private Category? _selectedCategory;
    public Category? SelectedCategory
    {
      get
      {
        IsDeleteCategoryButtonEnabled = ArePropertiesEqual(_selectedCategory, _oldCategory);
        IsDeleteTransactionButtonEnabled = ArePropertiesEqual(_selectedCategory, _oldTransaction?.Category);
        return _selectedCategory;
      }
      set
      {
        _selectedCategory = value;
        OnPropertyChanged();

        if (value?.Symbol == " ✚")
          InitCategoryPopup();
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
        transaction = new Transaction(string.Empty, null, 0, DateTime.Now, false);
      else
      {
        if (transaction.Amount < 0)
          transaction.Amount *= -1;
        else
          IsIncome = false;

        transaction.Recurrence ??= new("month", 1);
        _oldTransaction = new Transaction(transaction);
      }

      SelectedTransaction = transaction;
      GetCategories();
    }
    private void InitCategoryPopup(Category cat = null)
    {
      if (cat != null) //change (not yet) selected category
      {
        _oldCategory = new Category(cat.Name, cat.Symbol);
        SelectedCategory = cat;
      }
      else if (SelectedCategory?.Symbol == " ✚") //new category
        SelectedCategory = new Category(string.Empty, "❓");

      _view.OpenCategoryPopup();
    }

    private void SaveTransaction()
    {
      SelectedTransaction.Category = SelectedCategory;

      if (!IsIncome)
        SelectedTransaction.Amount *= -1;

      if (!SelectedTransaction.IsRecurring)
        SelectedTransaction.Recurrence = null;

      string msg = _model.SaveTransaction(SelectedTransaction, _oldTransaction);
      _view.CloseWindow();
    }
    private void SaveCategory()
    {
      string msg = _model.SaveCategory(SelectedCategory, _oldCategory);
      _view.CloseCategoryPopup();
      SelectedCategory = Categories[^2];
    }

    public void GetCategories()
    {
      Categories = new ObservableCollection<Category>();
      foreach (Category category in _model.GetCategories())
        Categories.Add(category);

      _oldCategory = null;
      if (SelectedTransaction.Category != null)
        SelectedCategory = Categories.FirstOrDefault(name => SelectedTransaction.Category.Name == name.Name);

      Categories.Add(new Category("Add new category", " ✚"));
    }

    private void DeleteTransaction()
    {
      if (!IsIncome)
        SelectedTransaction.Amount = SelectedTransaction.Amount * -1;

      if (!SelectedTransaction.IsRecurring)
        SelectedTransaction.Recurrence = null;

      string msg = _model.DeleteTransaction(SelectedTransaction);
      _view.CloseWindow();
    }
    private void DeleteCategory()
    {
      string msg = _model.DeleteCategory(SelectedCategory);
      _view.CloseCategoryPopup();
    }
    #endregion
  }
}