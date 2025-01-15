namespace ReturnToStonks
{
  public interface IModel
  {
    List<Transaction> GetTransactions();
    string SaveTransaction(Transaction selectedTransaction, Transaction? oldTransaction);
    string DeleteTransaction(Transaction selectedTransaction);

    List<Category> GetCategories();
    Category GetCategory(string name);
    string SaveCategory(Category selectedCategory, Category? oldCategory);
    string DeleteCategory(Category selectedCategory);
  }
}
