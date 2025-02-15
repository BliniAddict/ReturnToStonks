using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnToStonks
{
  class DebtViewModel : ViewModelBase
  {
    private Debt? _oldDebt;

    public DebtViewModel(IView view, IModel model, MessageService messageService, Debt? debt)
    {
      base.messageService = messageService;
      base.view = view;
      base.model = model;
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


  }
}
