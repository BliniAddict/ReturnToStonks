using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ReturnToStonks
{
  public class ViewModelBase : INotifyPropertyChanged
  {
    protected IView view;
    protected IModel model;
    protected MessageService messageService;

    #region PropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    #endregion

    #region Category
    protected Category? OldCategory;

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


    protected Category? _selectedCategory;
    public Category? SelectedCategory
    {
      get
      {
        CheckIfPropertyChanged();
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

    public virtual void CheckIfPropertyChanged() { }
    public virtual void GetCategories()
    {
      Categories = new ObservableCollection<Category>();
      foreach (Category category in model.GetCategories())
        Categories.Add(category);

      OldCategory = null;
      Categories.Add(new Category("Add new category", " ✚"));    
    }

    protected void InitCategoryPopup(Category cat = null)
    {
      if (cat != null) //change (not yet) selected category
      {
        OldCategory = new Category(cat);
        SelectedCategory = cat;
      }
      else if (SelectedCategory?.Symbol == " ✚") //new category
        SelectedCategory = new Category(string.Empty, "❓");

      view.OpenCategoryPopup();
    }

    protected void SaveCategory()
    {
      string message = model.SaveCategory(SelectedCategory, OldCategory);
      messageService.ShowMessage(message);

      view.CloseCategoryPopup();
      SelectedCategory = Categories[^2];
    }
    protected void DeleteCategory()
    {
      if (messageService.HasUserConfirmed("delete", SelectedCategory))
      {
        string message = model.DeleteCategory(SelectedCategory);
        messageService.ShowMessage(message);

        view.CloseCategoryPopup();
      }
    }
    #endregion
  }
}