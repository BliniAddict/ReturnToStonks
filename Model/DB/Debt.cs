namespace ReturnToStonks
{
  public class Debt
  {
    public Debt(Person? person, string purpose, Category? category, double amount, DateTime dueDate)
    {
      Person = person;
      Purpose = purpose;
      Category = category;
      Amount = amount;
      Due_date = dueDate;
    }
    public Debt(Debt debt)
    {
      Person = debt.Person == null ? null : new(debt.Person.First_Name, debt.Person.Last_Name, debt.Person.Contact_Method, debt.Person.Contact_ID);
      Purpose = debt.Purpose;
      Category = debt.Category == null ? null : new(debt.Category.Name, debt.Category.Symbol);
      Amount = debt.Amount;
      Due_date = debt.Due_date;
    }

    public Person? Person { get; set; }
    public string Purpose { get; set; }
    public Category? Category { get; set; }
    public double Amount { get; set; }
    public DateTime Due_date { get; set; }
  }
}
