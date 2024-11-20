using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
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

      ChosenTransaction = transaction;
      InitWindow();

      SaveCommand = new RelayCommand(Save);
    }


    public ICommand SaveCommand { get; }


    public Transaction ChosenTransaction { get; private set; }
    public ObservableCollection<Category> Categories { get; private set; }

    private Category _chosenCategory;
    public Category ChosenCategory
    {
      get => _chosenCategory;
      set
      {
        if (value.Name == "Add new category")
        {

        }
        else
          _chosenCategory = value;
        OnPropertyChanged();
      }
    }


    private void InitWindow()
    {
      Categories = new ObservableCollection<Category>();
      foreach (var category in _model.GetCategories())
        Categories.Add(category);
      Categories.Add(new Category("Add new category", '➕'));

      ChosenCategory = _model.GetCategory(ChosenTransaction.Category);
    }

    private void Save()
    {
      throw new NotImplementedException();
    }
  }
}
