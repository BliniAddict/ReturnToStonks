using CommunityToolkit.Mvvm.Input;
using ReturnToStonks.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

      SaveCommand = new RelayCommand(Save);
    }
    public ICommand SaveCommand { get; }

    public Transaction ChosenTransaction { get; private set; }

    private void Save()
    {
      throw new NotImplementedException();
    }
  }
}
