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

    public TransactionViewModel(IView view, IModel model, Transaction transaction)
    {
      _view = view;
      _model = model;

      SelectedTransaction = transaction;
      GetCategories();

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
    private Category? _oldCategory;

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
      //string msg = _model.SaveTransaction(SelectedTransaction, _oldTransaction);
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
      foreach (var category in _model.GetCategories())
        Categories.Add(category);

      _oldCategory = null;
      SelectedCategory = _model.GetCategory(SelectedTransaction.Category);

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
