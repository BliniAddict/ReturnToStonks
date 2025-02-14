using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnToStonks
{
  class DebtViewModel : ViewModelBase
  {
    private readonly MessageService _messageService;
    private readonly IView _view;
    private readonly IModel _model;

    private Transaction? _oldTransaction;
    private Category? _oldCategory;

    public DebtViewModel(IView view, IModel model, MessageService messageService, Debt? debt)
    {
      _messageService = messageService;
      _view = view;
      _model = model;
    }


    private Debt _selectedDebt;
    public Debt SelectedDebt
    {
      get => _selectedDebt;
      set
      {
        _selectedDebt = value;
        OnPropertyChanged();
      }
    }
    private bool _isOwedToMe;
    public bool IsOwedToMe
    {
      get => _isOwedToMe;
      set
      {
        _isOwedToMe = value;
        OnPropertyChanged();
      }
    }

    private Category? _selectedCategory;
    public Category? SelectedCategory
    {
      get
      {
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
  }
}
