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
      InitCategories();

      SaveTransactionCommand = new RelayCommand(SaveTransaction);
      SaveCategoryCommand = new RelayCommand(SaveCategory);
      ChangeCategoryCommand = new RelayCommand<Category>(InitCategoryPopup);
      DeleteCategoryCommand = new RelayCommand(DeleteCategory);
    }


    public ICommand SaveTransactionCommand { get; }
    public ICommand SaveCategoryCommand { get; }
    public ICommand ChangeCategoryCommand { get; }
    public ICommand DeleteCategoryCommand { get; }


    public Transaction SelectedTransaction { get; private set; }

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

    private Category _selectedCategory;
    public Category SelectedCategory
    {
      get => _selectedCategory;
      set
      {
        _selectedCategory = value;
        OnPropertyChanged();

        if (value?.Symbol == '✚')
          InitCategoryPopup();
      }
    }
    private Category _oldCategory;

    public void InitCategories()
    {
      Categories = new ObservableCollection<Category>();
      foreach (var category in _model.GetCategories())
        Categories.Add(category);

      SelectedCategory = _model.GetCategory(SelectedTransaction.Category);

      Categories.Add(new Category("Add new category", '✚'));
    }
    private void InitCategoryPopup(Category cat = null)
    {
      if (cat != null)
        SelectedCategory = cat;
      if (SelectedCategory?.Symbol == '✚')
        SelectedCategory = new Category(string.Empty, '❓');
      _view.OpenCategoryPopup();
    }

    private void SaveTransaction()
    {
      throw new NotImplementedException();
    }
    private void SaveCategory()
    {
      //TODO: Kontrolle, dass wenigstens eins von denen nicht NULL/empty sind
      string msg = _model.SaveCategory(SelectedCategory);
      _view.CloseCategoryPopup();
      SelectedCategory = Categories[^2];
    }
    private void DeleteCategory()
    {
      //TODO: Bei geänderten Categories, die nicht in der DB vorhanden sind: delete button disablen
      string msg = _model.DeleteCategory(SelectedCategory);
      _view.CloseCategoryPopup();
    }
  }
}
