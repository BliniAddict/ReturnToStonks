using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReturnToStonks
{
  public class Transaction : IModel
  {
    public Transaction(double amount, string category, bool isRecurring, DateTime date, string purpose)
    {
      Amount = amount;
      Category = category;
      IsRecurring = isRecurring;
      Date = date;
      Purpose = purpose;
    }

    public double Amount {  get; set; }
    public string Category { get; set; }
    public bool IsRecurring { get; set; }
    public DateTime Date { get; set; }
    public string Purpose { get; set; }
  }
}
