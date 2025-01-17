using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.IO;
using System.Reflection;
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
    private static SqliteParameter CreateParameter(string name, object? value)
    {
      return new SqliteParameter(name, value ?? DBNull.Value);
    }
    private List<string> GetDeleteConditions<T>(T row)
    {
      List<string> conditions = new();

      using (var command = _connection.CreateCommand())
      {
        PropertyInfo[] properties = row.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties.Where(noClass => noClass.PropertyType == typeof(string) || !noClass.PropertyType.IsClass))
        {
          object value = property.GetValue(row);

          if (value == null)
            conditions.Add(property.Name.ToLower() + " is NULL");

          else if (property.PropertyType == typeof(string))
            conditions.Add($"{property.Name.ToLower()}='{value}'");

          else if (property.PropertyType == typeof(DateTime))
            conditions.Add($"{property.Name.ToLower()}='{Convert.ToDateTime(value):yyyy-MM-dd}'");

          else if (property.PropertyType == typeof(double))
            conditions.Add($"{property.Name.ToLower()}={Convert.ToDouble(value).ToString("0.0", CultureInfo.InvariantCulture)}");

          else if (property.PropertyType.IsPrimitive)
            conditions.Add($"{property.Name.ToLower()}={value}");
        }
      }

      return conditions;
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
          command.Parameters.Add(CreateParameter("@oldPurpose", oldTransaction.Purpose));
          command.Parameters.Add(CreateParameter("@oldCategory", oldTransaction.Category?.Name));
          command.Parameters.Add(CreateParameter("@oldAmount", oldTransaction.Amount));
          command.Parameters.Add(CreateParameter("@oldDate", oldTransaction.Date.ToString("yyyy-MM-dd")));
          command.Parameters.Add(CreateParameter("@oldRecurrence_Span", oldTransaction.Recurrence?.Span ?? 0));
          command.Parameters.Add(CreateParameter("@oldRecurrence_Unit", oldTransaction.Recurrence?.Unit));
        }
        command.Parameters.Add(CreateParameter("@newPurpose", selectedTransaction.Purpose));
        command.Parameters.Add(CreateParameter("@newCategory", selectedTransaction.Category?.Name));
        command.Parameters.Add(CreateParameter("@newAmount", selectedTransaction.Amount));
        command.Parameters.Add(CreateParameter("@newDate", selectedTransaction.Date.ToString("yyyy-MM-dd")));
        command.Parameters.Add(CreateParameter("@newRecurrence_Span", selectedTransaction.Recurrence?.Span ?? 0));
        command.Parameters.Add(CreateParameter("@newRecurrence_Unit", selectedTransaction.Recurrence?.Unit));

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
            reader.IsDBNull(1) ? null : GetCategory(reader.GetString(1)),
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
    public string DeleteTransaction(Transaction selectedTransaction)
    {
      string result = string.Empty;

      using (var command = _connection.CreateCommand())
      {
        List<string> conditions = GetDeleteConditions(selectedTransaction);

        //remove unwanted conditions
        conditions.Remove(conditions.First(r => r.Contains("isrecurring")));

        //add conditions associated with class properties
        if (selectedTransaction.Category?.Name == null)
          conditions.Add("category IS NULL");
        else
          conditions.Add($"category={selectedTransaction.Category.Name}");

        conditions.Add($"recurrence_span={selectedTransaction.Recurrence?.Span ?? 0}");
        if (selectedTransaction.Recurrence == null)
          conditions.Add($"recurrence_unit IS NULL");
        else
          conditions.Add($"recurrence_unit={selectedTransaction.Recurrence?.Unit ?? "''"}");

        command.CommandText = "DELETE FROM Transactions WHERE " + string.Join(" AND ", conditions);
        int rowsAffected = command.ExecuteNonQuery();

        result = rowsAffected > 0 ? "Transaction deleted successfully" : "No rows affected. Delete failed.";
      }
      return result;
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
          command.Parameters.Add(CreateParameter("@oldName", oldCategory.Name));
          command.Parameters.Add(CreateParameter("@oldSymbol", oldCategory.Symbol));
        }
        command.Parameters.Add(CreateParameter("@newName", selectedCategory.Name));
        command.Parameters.Add(CreateParameter("@newSymbol", selectedCategory.Symbol));

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

      command.Parameters.Add(CreateParameter("@name", name));

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
        List<string> conditions = GetDeleteConditions(selectedCategory);
        command.CommandText = "DELETE FROM Categories WHERE " + string.Join(" AND ", conditions);

        int rowsAffected = command.ExecuteNonQuery();
        result = rowsAffected > 0 ? "Category deleted successfully" : "No rows affected. Save failed.";
      }

      return result;
    }
    #endregion
  }
}