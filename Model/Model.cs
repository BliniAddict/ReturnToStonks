using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace ReturnToStonks
{
  public class Model : IModel
  {
    private readonly SqliteConnection _connection;
    public Model()
    {
      _connection = new SqliteConnection(@"Data Source=..\..\..\Model\DB\Data.db");
      _connection.Open();
    }

    #region Transactions
    public string SaveTransaction(Transaction selectedTransaction, Transaction? oldTransaction)
    {
      string result = string.Empty;

      using (var command = _connection.CreateCommand())
      {
        if (oldTransaction == null) //INSERT
          command.CommandText = "INSERT INTO Transactions (purpose, category, amount, date, recurrence_span, recurrence_unit) " +
            "VALUES (@newPurpose, @newCategory, @newAmount, @newDate, @newRecurrence_Span, @newRecurrence_Unit)";
        else //UPDATE
        {
          command.CommandText = "UPDATE Transactions SET purpose=@newPurpose, category=@newCategory, amount=@newAmount, date=@newDate, recurrence_span=@newRecurrence_Span, recurrence_Unit=@newRecurrence_Unit " +
            "WHERE purpose=@oldPurpose AND category=@oldCategory AND amount=@oldAmount AND date=@oldDate AND recurrence_span=@oldRecurrence_Span AND recurrence_Unit=@oldRecurrence_Unit";
          command.Parameters.AddWithValue("@oldPurpose", oldTransaction.Purpose);
          command.Parameters.AddWithValue("@oldCategory", oldTransaction.Category?.Name ?? string.Empty);
          command.Parameters.AddWithValue("@oldAmount", oldTransaction.Amount);
          command.Parameters.AddWithValue("@oldDate", oldTransaction.Date.ToString("yyyy-MM-dd"));
          command.Parameters.AddWithValue("@oldRecurrence_Span", oldTransaction.Recurrence?.SelectedSpan ?? 0);
          command.Parameters.AddWithValue("@oldRecurrence_Unit", oldTransaction.Recurrence?.SelectedUnit ?? string.Empty);
        }
        command.Parameters.AddWithValue("@newPurpose", selectedTransaction.Purpose);
        command.Parameters.AddWithValue("@newCategory", selectedTransaction.Category?.Name ?? string.Empty);
        command.Parameters.AddWithValue("@newAmount", selectedTransaction.Amount);
        command.Parameters.AddWithValue("@newDate", selectedTransaction.Date.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@newRecurrence_Span", selectedTransaction.Recurrence?.SelectedSpan ?? 0);
        command.Parameters.AddWithValue("@newRecurrence_Unit", selectedTransaction.Recurrence?.SelectedUnit ?? string.Empty);

        int rowsAffected = command.ExecuteNonQuery();
        result = rowsAffected > 0 ? "Transaction saved successfully" : "No rows affected. Save failed.";
      }

      return result;
    }
    public List<Transaction> GetTransactions()
    {
      List<Transaction> res = new();

      using var command = _connection.CreateCommand();
      command.CommandText = "SELECT purpose, category, amount, date, recurrence_span, recurrence_unit FROM Transactions";

      using (var reader = command.ExecuteReader())
      {
        while (reader.Read())
        {
          Transaction tr = new(
            reader.GetString(0),
            GetCategory(reader.GetString(1)),
            reader.GetDouble(2),
            DateTime.ParseExact(reader.GetString(3), "yyyy-MM-dd", CultureInfo.InvariantCulture),
            reader.GetInt32(4) != 0);

          if (tr.IsRecurring)
            tr.Recurrence = new(reader.GetString(5), reader.GetInt32(4));

          res.Add(tr);            
        }
      }

      return res;
    }
    #endregion

    #region Categories
    public string SaveCategory(Category selectedCategory, Category? oldCategory)
    {
      string result = string.Empty;

      using (var command = _connection.CreateCommand())
      {
        if (oldCategory == null) //INSERT
          command.CommandText = "INSERT INTO Categories (name, symbol) VALUES (@newName, @newSymbol)";
        else //UPDATE
        {
          command.CommandText = "UPDATE Categories SET name=@newName, symbol=@newSymbol WHERE name=@oldName AND symbol=@oldSymbol";
          command.Parameters.AddWithValue("@oldName", oldCategory.Name);
          command.Parameters.AddWithValue("@oldSymbol", oldCategory.Symbol);
        }
        command.Parameters.AddWithValue("@newName", selectedCategory.Name);
        command.Parameters.AddWithValue("@newSymbol", selectedCategory.Symbol);

        int rowsAffected = command.ExecuteNonQuery();
        result = rowsAffected > 0 ? "Category saved successfully" : "No rows affected. Save failed.";
      }

      return result;
    }

    public List<Category> GetCategories()
    {
      List<Category> res = new();

      using var command = _connection.CreateCommand();
      command.CommandText = "SELECT name, symbol FROM Categories";

      using (var reader = command.ExecuteReader())
      {
        while (reader.Read())
          res.Add(new Category(reader.GetString(0), reader.GetString(1)));
      }

      return res;
    }
    public Category GetCategory(string name)
    {
      using var command = _connection.CreateCommand();
      command.CommandText = "SELECT name, symbol FROM Categories WHERE name = @name";

      command.Parameters.AddWithValue("@name", name);

      using var reader = command.ExecuteReader();

      if (reader.Read())
        return new Category(reader.GetString(0), reader.GetString(1));

      return null;
    }

    public string DeleteCategory(Category selectedCategory)
    {
      string result = string.Empty;

      using (var command = _connection.CreateCommand())
      {
        command.CommandText = "DELETE FROM Categories WHERE name=@name";
        command.Parameters.AddWithValue("@name", selectedCategory.Name);

        int rowsAffected = command.ExecuteNonQuery();
        result = rowsAffected > 0 ? "Category deleted successfully" : "No rows affected. Save failed.";
      }

      return result;
    }
    #endregion
  }
}