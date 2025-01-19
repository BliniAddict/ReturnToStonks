namespace ReturnToStonks
{
  public interface IView
  {
    void CloseWindow();
    void ShowMessage(string message);

    virtual void CloseCategoryPopup() { }
    virtual void OpenCategoryPopup() { }
  }
}
