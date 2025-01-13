namespace ReturnToStonks
{
  public interface IView
  {
    void CloseWindow();

    virtual void CloseCategoryPopup() { }
    virtual void OpenCategoryPopup() { }
  }
}
