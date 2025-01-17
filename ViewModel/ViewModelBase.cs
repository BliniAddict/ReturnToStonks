using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ReturnToStonks
{
  public class ViewModelBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    protected bool ArePropertiesEqual<T>(T obj1, T obj2)
    {
      if (obj1 == null ^ obj2 == null)
        return false;
      if ((obj1 == null && obj2 == null) || ReferenceEquals(obj1, obj2))
        return true;

      Type type = obj1.GetType();
      PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
      foreach (var property in properties)
      {
        object value1 = property.GetValue(obj1);
        object value2 = property.GetValue(obj2);

        if (value1 == null ^ value2 == null)
          return false;
        else
        {
          if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
            continue;
          if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string) || !property.PropertyType.IsClass)
          {
            if (!Equals(value1, value2))
              return false;
          }
          else
          {
            if (!ArePropertiesEqual(value1, value2))
              return false;
          }
        }
      }
      return true;
    }
  }
}