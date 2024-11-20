namespace ReturnToStonks
{
  public class Category
  {
    public Category(string name, Char? symbol = null)
    {
      Name = name;
      Symbol = symbol;
    }

    public string Name { get; set; }
    public Char? Symbol { get; set; }
  }
}
