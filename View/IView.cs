using ReturnToStonks.View.Controls;
using System.Windows;
using System.Windows.Controls;

namespace ReturnToStonks
{
  public interface IView
  {
    void CloseWindow(Window window) => window.Close();
    void ShowMessage(string message)
    {
      if (this is Window window && window.Content is Grid grid)
      {
        NotificationPopup notification = new()
        {
          Width = 300,
          HorizontalAlignment = HorizontalAlignment.Center,
          VerticalAlignment = VerticalAlignment.Bottom,
          Margin = new Thickness(0, 0, 0, 20)
        };

        grid.Children.Add(notification);
        grid.Children[^1].SetValue(Grid.ColumnSpanProperty, 10);
        grid.Children[^1].SetValue(Grid.RowSpanProperty, 10);

        notification.ShowMessage(message);

        Task.Delay(3000).ContinueWith(_ =>
        {
          grid.Dispatcher.Invoke(() => { grid.Children.Remove(notification); });
        });
      }
    }

    virtual void CloseCategoryPopup() { }
    virtual void OpenCategoryPopup() { }
  }
}
