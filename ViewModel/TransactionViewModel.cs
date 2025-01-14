using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ReturnToStonks
{
  public class TransactionViewModel : ViewModelBase
  {
    private IView _view;
    private IModel _model;

    public TransactionViewModel(IView view, IModel model, Transaction? transaction)
    {
      _view = view;
      _model = model;

      InitTransactionWindow(transaction);

      SaveTransactionCommand = new RelayCommand(SaveTransaction);
      SaveCategoryCommand = new RelayCommand(SaveCategory);
      ChangeCategoryCommand = new RelayCommand<Category>(InitCategoryPopup);
      DeleteCategoryCommand = new RelayCommand(DeleteCategory);
    }

    public ICommand SaveTransactionCommand { get; }

    public ICommand SaveCategoryCommand { get; }
    public ICommand ChangeCategoryCommand { get; }
    public ICommand DeleteCategoryCommand { get; }

    #region Properties
    public Transaction SelectedTransaction { get; private set; }
    private Transaction? _oldTransaction;

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

    private Category? _oldCategory;
    private Category? _selectedCategory;
    public Category? SelectedCategory
    {
      get
      {
        IsDeleteButtonEnabled = _selectedCategory?.Name.Trim() == _oldCategory?.Name.Trim() && _selectedCategory?.Symbol == _oldCategory?.Symbol;
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

    public bool IsIncome { get; set; } = false;

    private bool _isDeleteButtonEnabled;
    public bool IsDeleteButtonEnabled
    {
      get => _isDeleteButtonEnabled;
      set
      {
        _isDeleteButtonEnabled = value;
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
        _oldTransaction = new Transaction(transaction);

      SelectedTransaction = transaction;

      if (SelectedTransaction.Amount < 0)
        SelectedTransaction.Amount = SelectedTransaction.Amount * -1;
      else
        IsIncome = false;

      if (SelectedTransaction.Recurrence == null)
        SelectedTransaction.Recurrence = new("month", 1);

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
        SelectedTransaction.Amount = SelectedTransaction.Amount * -1;

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

    private void DeleteCategory()
    {
      string msg = _model.DeleteCategory(SelectedCategory);
      _view.CloseCategoryPopup();
    }
    #endregion
  }
}
