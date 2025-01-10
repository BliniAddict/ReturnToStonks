namespace ReturnToStonks
{
  public interface IModel
  {
    List<Transaction> GetTransactions();
    string SaveTransaction(Transaction selectedTransaction, Transaction? oldTransaction);

    List<Category> GetCategories();
    Category GetCategory(string name);
    string SaveCategory(Category selectedCategory, Category? oldCategory);
    string DeleteCategory(Category selectedCategory);
  }
}
