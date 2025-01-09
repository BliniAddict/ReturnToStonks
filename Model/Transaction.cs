namespace ReturnToStonks
{
  public class Transaction
  {
    public Transaction(double amount, string category, bool isRecurring, DateTime date, string purpose, Recurrence recurrence = null)
    {
      Amount = amount;
      Category = category;
      IsRecurring = isRecurring;
      Date = date;
      Purpose = purpose;
      Recurrence = recurrence ?? new Recurrence("month", 1);
    }

    public double Amount {  get; set; }
    public string Category { get; set; }
    public bool IsRecurring { get; set; }
    public Recurrence Recurrence { get; set; }
    public DateTime Date { get; set; }
    public string Purpose { get; set; }
  }
}
