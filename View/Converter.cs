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
  #region Input
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
        string result = string.Empty;

        string removedComma = input.Replace(",", string.Empty);
        MatchCollection matches = Regex.Matches(removedComma, @"\d+(\.\d{1,2})?");

        for (int i = 0; i < matches.Count; i++)
        {
          if (i > 0)
            result += matches[i].Value.Replace(".", string.Empty);
          else
            result += matches[i].Value;
        }
        return result;
      }
      return input;
    }
  }
  #endregion

  #region Visual Converters
  public class DateOutputConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is DateTime date)
        return date.ToString("d", new CultureInfo("en-US"));
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class PluralWordingConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      var selectedSpan = (int)values[1];

      if (values[0] is ObservableCollection<string> wordsList)
        return wordsList.Select(unit => selectedSpan > 1 ? unit + "s" : unit).ToList();
      else if (values[0] is string word)
          return selectedSpan > 1 ? word + "s" : word.Replace("s", string.Empty);
      return values[0];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return new object[] { value };
    }
  }
  #endregion

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

}