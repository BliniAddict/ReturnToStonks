using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ReturnToStonks
{
  public partial class UpDownCounter : UserControl
  {
    public UpDownCounter()
    {
      InitializeComponent();
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(int?), typeof(UpDownCounter), new PropertyMetadata(null));

    public int? Text
    {
      get { return (int?)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
      if (Text < 999999999)
        Text++;
    }

    private void SubtractButton_Click(object sender, RoutedEventArgs e)
    {
      if (Text > 1)
        Text--;
    }

    private void txtValue_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
      TextBox textBox = sender as TextBox;
      string fullText = GetProposedText(textBox, e.Text);

      e.Handled = !Regex.IsMatch(fullText, "^[1-9]*$");
    }


    private static string GetProposedText(TextBox textBox, string input)
    {
      int selectionStart = textBox.SelectionStart;
      int selectionLength = textBox.SelectionLength;
      string currentText = textBox.Text;

      return currentText.Remove(selectionStart, selectionLength).Insert(selectionStart, input);
    }
  }
}
