namespace ReturnToStonks
{
  public interface IModel
  {
    List<Transaction> GetTransactions(DateTime? minDate = null);
    string SaveTransaction(Transaction selectedTransaction, Transaction? oldTransaction = null);
    string DeleteTransaction(Transaction selectedTransaction);

    List<Category> GetCategories();
    Category GetCategory(string name);
    string SaveCategory(Category selectedCategory, Category? oldCategory = null);
    string DeleteCategory(Category selectedCategory);
  }
}
