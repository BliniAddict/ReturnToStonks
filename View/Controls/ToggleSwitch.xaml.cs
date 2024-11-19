using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReturnToStonks
{
  public partial class ToggleSwitch : UserControl
  {
    public ToggleSwitch()
    {
      InitializeComponent();
      Loaded += (s, e) => UpdateText();
    }

    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleSwitch), new PropertyMetadata(false, OnIsCheckedChanged));

    public bool IsChecked
    {
      get { return (bool)GetValue(IsCheckedProperty); }
      set { SetValue(IsCheckedProperty, value); }
    }

    #region Text
    private static readonly DependencyPropertyKey TextPropertyKey =
        DependencyProperty.RegisterReadOnly("Text", typeof(string), typeof(ToggleSwitch), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty TextProperty = TextPropertyKey.DependencyProperty;

    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      private set { SetValue(TextPropertyKey, value); }
    }

    public static readonly DependencyProperty CheckedTextProperty =
        DependencyProperty.Register("CheckedText", typeof(string), typeof(ToggleSwitch), new PropertyMetadata("On"));

    public string CheckedText
    {
      get { return (string)GetValue(CheckedTextProperty); }
      set { SetValue(CheckedTextProperty, value); }
    }

    public static readonly DependencyProperty UncheckedTextProperty =
        DependencyProperty.Register("UncheckedText", typeof(string), typeof(ToggleSwitch), new PropertyMetadata("Off"));

    public string UncheckedText
    {
      get { return (string)GetValue(UncheckedTextProperty); }
      set { SetValue(UncheckedTextProperty, value); }
    }

    private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (ToggleSwitch)d;
      control.UpdateText();
    }

    private void UpdateText()
    {
      Text = IsChecked ? CheckedText : UncheckedText;
    }
    #endregion
  }
}
