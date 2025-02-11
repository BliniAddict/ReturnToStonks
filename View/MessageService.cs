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

      _views.LastOrDefault().ShowMessage(message);
    }
  }
}