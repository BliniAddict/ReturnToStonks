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

    public Recurrence(string unit, int span)
    {
      Unit = unit;
      Span = span;
    }

    public ObservableCollection<string> UnitsList { get; } = new ObservableCollection<string> { "day", "week", "month", "year" };

    public string Unit { get; set; }

    private int _span;
    public int Span
    {
      get => _span;
      set
      {
        _span = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Span)));
      }
    }
  }
}
