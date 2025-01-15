﻿namespace ReturnToStonks
{
  public class Transaction
  {
    public Transaction(Transaction transaction)
    {
      Purpose = transaction.Purpose;
      Category = transaction.Category == null ? null : new(transaction.Category.Name, transaction.Category.Symbol);
      Amount = transaction.Amount;
      Date = transaction.Date;
      IsRecurring = transaction.IsRecurring;
      Recurrence = transaction.Recurrence == null ? null : new(transaction.Recurrence.SelectedUnit, transaction.Recurrence.SelectedSpan);
    }

    public Transaction(string purpose, Category? category, double amount, DateTime date, bool isRecurring, Recurrence recurrence = null)
    {
      Purpose = purpose;
      Category = category;
      Amount = amount;
      Date = date;
      IsRecurring = isRecurring;
      Recurrence = recurrence;
    }

    public string Purpose { get; set; }
    public Category? Category { get; set; }
    public double Amount {  get; set; }
    public DateTime Date { get; set; }
    public bool IsRecurring { get; set; }
    public Recurrence? Recurrence { get; set; }
  }
}
