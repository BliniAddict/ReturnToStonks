﻿using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ReturnToStonks
{
  #region Input
  public class MonetaryInputConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value;

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

  #region Output
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
      if (values[1] is int span)
      {
        if (values[0] is ObservableCollection<string> wordsList)
          return wordsList.Select(unit => span > 1 ? unit + "s" : unit).ToList();
        if (values[0] is string word)
          return span > 1 ? word + "s" : word.Replace("s", string.Empty);
      }
      return values[0];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => new object[] { value };
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

  public class IsTrueVisibilityConverter : IValueConverter
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

  #region Color
  public class CellBackgroundConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (DateTime.TryParseExact(value.ToString(), "d", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date))
      {
        if (date >= DateTime.Today)
          return Application.Current.Resources["TertiaryColor_Light"] as SolidColorBrush;
      }
      return Application.Current.Resources["SecondaryColor_Light"] as SolidColorBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
  #endregion
}