using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ReturnToStonks
{
  public class MonetaryInputConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string? input = value as string;

      if (!string.IsNullOrWhiteSpace(input))
      {
        string cleanedInput = input.Replace(",", string.Empty);
        string match = Regex.Match(cleanedInput, @"\d+(\.\d{1,2})?").Value;
        return double.Parse(match, CultureInfo.InvariantCulture);
      }
      return input;
    }
  }

  #region Visibility
  public class ComboboxButtonVisibilityConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length == 2 && values[0] is string symbol && values[1] is bool isMouseOver)
      {
        if (symbol != " ✚" && isMouseOver)
          return Visibility.Visible;
      }
      return Visibility.Collapsed;
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class ControlsVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool isRecurring && isRecurring)
        return Visibility.Visible;
      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
  #endregion

  public class PluralWordingConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var selectedSpan = (int)values[1];

      if (values[0] is ObservableCollection<string> wordsList)
        return wordsList.Select(unit => selectedSpan > 1 ? unit + "s" : unit).ToList();
      else if (values[0] is string word)
      {
        if (!string.IsNullOrWhiteSpace(word))
          return selectedSpan > 1 ? word + "s" : word;
      }
      return values[0];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return new object[] { value };
    }
  }
}