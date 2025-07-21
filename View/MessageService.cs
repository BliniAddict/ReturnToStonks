using System.Windows;

namespace ReturnToStonks
{
    public class MessageService
    {
        private readonly List<IView> _views = new List<IView>();

        public void RegisterView(IView view)
        {
            if (!_views.Contains(view))
                _views.Add(view);
        }
        public void UnregisterView(IView view)
        {
            if (_views.Contains(view))
                _views.Remove(view);
        }

        public void ShowMessage(string message, bool showOnMainWindow = false)
        {
            if (showOnMainWindow && _views.Count > 1)
                _views.Remove(_views.LastOrDefault());

            _views.LastOrDefault().ShowToastMessage(message);
        }

        public bool HasUserConfirmed<T>(string option, T type, string? additionalMessage = null)
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