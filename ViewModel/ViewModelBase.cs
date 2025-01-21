using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReturnToStonks
{
  public class ViewModelBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    protected bool HasUserConfirmed<T>(string option, T type, string? additionalMessage = null)
    {
      string caption = "Warning";
      MessageBoxButton button = MessageBoxButton.YesNo;
      MessageBoxImage icon = MessageBoxImage.Warning;
      string messageBoxText = $"Are you sure you want to {option} this {type.GetType().Name}?";
      if (!string.IsNullOrWhiteSpace(additionalMessage))
        messageBoxText += "\n" + additionalMessage;

      string res = MessageBox.Show(messageBoxText, caption, button, icon).ToString();
      return res.ToLower() == "yes";
    }
  }
}