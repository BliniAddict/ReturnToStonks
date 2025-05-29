using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace ReturnToStonks.View.Controls
{
  /// <summary>
  /// Interaction logic for NotificationPopup.xaml
  /// </summary>
  public partial class NotificationPopup : UserControl
  {
    private string _notificationMessage;
    private double _notificationOpacity;
    private System.Timers.Timer _fadeOutTimer;

    public NotificationPopup()
    {
      InitializeComponent();
      DataContext = this;
      NotificationOpacity = 0.8;

      _fadeOutTimer = new System.Timers.Timer(1000);
      _fadeOutTimer.Elapsed += FadeOut;
    }

    public string NotificationMessage
    {
      get => _notificationMessage;
      set
      {
        _notificationMessage = value;
        OnPropertyChanged(nameof(NotificationMessage));
      }
    }
    public double NotificationOpacity
    {
      get => _notificationOpacity;
      set
      {
        _notificationOpacity = value;
        OnPropertyChanged(nameof(NotificationOpacity));
      }
    }

    public void ShowMessage(string message)
    {
      NotificationMessage = message;
      _fadeOutTimer.Start();
    }
    private void FadeOut(object sender, ElapsedEventArgs e)
    {
      _fadeOutTimer.Stop();

      Application.Current.Dispatcher.Invoke(() =>
      {
        var fadeOutDuration = TimeSpan.FromSeconds(1);
        var fadeOutAnimation = new System.Windows.Media.Animation.DoubleAnimation(0.8, 0.0, fadeOutDuration);
        fadeOutAnimation.Completed += (s, _) => NotificationMessage = string.Empty;
        this.BeginAnimation(UserControl.OpacityProperty, fadeOutAnimation);
      });
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}
