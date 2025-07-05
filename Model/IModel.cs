namespace ReturnToStonks
{
    public interface IModel
    {
        List<Transaction> GetTransactions();
        string SaveTransaction(Transaction selectedTransaction, Transaction? oldTransaction = null);
        string DeleteTransaction(Transaction selectedTransaction);

        string SaveDebt(Debt selectedDebt, Debt? oldDebt);

        List<Category> GetCategories();
        Category GetCategory(string name);
        string SaveCategory(Category selectedCategory, Category? oldCategory = null);
        string DeleteCategory(Category selectedCategory);

        List<Person> GetPersons();
        string SavePerson(Person selectedPerson, Person? oldPerson = null);
        string DeletePerson(Person selectedPerson);
    }
}
