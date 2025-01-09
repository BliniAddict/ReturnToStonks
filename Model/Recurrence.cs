using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReturnToStonks
{
  public class Recurrence : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public Recurrence(string selectedUnit, int selectedNumber)
    {
      SelectedUnit = selectedUnit;
      SelectedSpan = selectedNumber;
    }

    public ObservableCollection<string> UnitsList { get; } = new ObservableCollection<string> { "day", "week", "month", "year" };

    public string SelectedUnit { get; set; }
    private int _selectedSpan;
    public int SelectedSpan
    {
      get => _selectedSpan;
      set
      {
        _selectedSpan = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSpan)));
      }
    }
  }
}
