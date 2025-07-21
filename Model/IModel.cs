namespace ReturnToStonks
{
    public interface IModel
    {
        List<Transaction> GetTransactions();
        string SaveTransaction(Transaction selectedTransaction, Transaction? oldTransaction = null);
        string DeleteTransaction(Transaction selectedTransaction);

        List<Debt> GetDebts();
        string SaveDebt(Debt selectedDebt, Debt? oldDebt);
        string DeleteDebt(Debt selectedDebt);

        List<Category> GetCategories();
        string SaveCategory(Category selectedCategory, Category? oldCategory = null);
        string DeleteCategory(Category selectedCategory);

        List<Person> GetPersons();
        string SavePerson(Person selectedPerson, Person? oldPerson = null);
        string DeletePerson(Person selectedPerson);
    }
}
