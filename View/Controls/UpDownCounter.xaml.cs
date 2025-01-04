using System;
using System.Collections.Generic;
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
  /// <summary>
  /// Interaction logic for UpDownCounter.xaml
  /// </summary>
  public partial class UpDownCounter : UserControl
  {
    public UpDownCounter()
    {
      InitializeComponent();
    }
    // Property to get or set the value
    public int Value
    {
      get
      {
        if (int.TryParse(txtValue.Text, out int result))
          return result;
        return 0;
      }
      set
      {
        txtValue.Text = value.ToString();
      }
    }

    // Add button click handler
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
      Value++;
    }

    // Subtract button click handler
    private void SubtractButton_Click(object sender, RoutedEventArgs e)
    {
      Value--;
    }

    // Optional: Ensure only numbers are entered
    private void TxtValue_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!int.TryParse(txtValue.Text, out _))
      {
        txtValue.Text = "0"; // Reset to default if input is invalid
      }
    }
  }
}
