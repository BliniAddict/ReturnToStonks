using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReturnToStonks
{
  public class MainViewModel : INotifyPropertyChanged
  {
    #region essential 4 properties
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    #endregion

    public MainViewModel()
    {

      AddTransactionCommand = new RelayCommand(AddTransaction);
    }

    public ICommand AddTransactionCommand { get; }

    private void AddTransaction()
    {

    }
  }
}
