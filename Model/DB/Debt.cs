namespace ReturnToStonks
{
    public class Debt
    {
        public Debt(string purpose, Person? person, Category? category, double amount, DateTime? dueDate = null, bool owedToMe = false)
        {
            Purpose = purpose;
            Person = person;
            Category = category;
            Amount = amount;
            Due_date = dueDate;
        }
        public Debt(Debt debt)
        {
            Purpose = debt.Purpose;
            Person = debt.Person == null ? null : new(debt.Person.Name, debt.Person.Contact_Method, debt.Person.Contact_ID);
            Category = debt.Category == null ? null : new(debt.Category.Name, debt.Category.Symbol);
            Amount = debt.Amount;
            Due_date = debt.Due_date;
        }

        public Person? Person { get; set; }
        public string Purpose { get; set; }
        public Category? Category { get; set; }
        public double Amount { get; set; }
        public DateTime? Due_date { get; set; }
    }
}
