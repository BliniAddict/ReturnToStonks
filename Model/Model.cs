using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace ReturnToStonks
{
  public class Model : IModel
  {
    private readonly SqliteConnection _connection;
    public Model()
    {
      _connection = new SqliteConnection(@"Data Source=..\..\..\Model\Data.db");
      _connection.Open();
    }


    public List<Category> GetCategories()
    {
      List<Category> res = new();

      using var command = _connection.CreateCommand();
      command.CommandText = "SELECT name, symbol FROM Categories";

      using (var reader = command.ExecuteReader())
      {
        while (reader.Read())
          res.Add(new Category(reader.GetString(0), Convert.ToChar(reader.GetString(1))));
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
        return new Category(reader.GetString(0), Convert.ToChar(reader.GetString(1)));

      return null;
    }


    public string SaveCategory(Category selectedCategory, Category oldCategory = null)
    {
      string result;

      using (var command = _connection.CreateCommand())
      {
        #region INSERT
        command.CommandText = "INSERT INTO Categories (name, symbol) VALUES (@name, @symbol)";
        command.Parameters.AddWithValue("@name", selectedCategory.Name);
        command.Parameters.AddWithValue("@symbol", selectedCategory.Symbol);

        int rowsAffected = command.ExecuteNonQuery();
        result = rowsAffected > 0 ? "Category saved successfully" : "No rows affected. Save failed.";
        #endregion
      }

      return result;
    }

    public string DeleteCategory(Category selectedCategory)
    {
      string result;

      using (var command = _connection.CreateCommand())
      {
        command.CommandText = "DELETE FROM Categories WHERE name=@name";
        command.Parameters.AddWithValue("@name", selectedCategory.Name);
        //TODO: vielleicht kann man ja beide columns zum PK machen?

        int rowsAffected = command.ExecuteNonQuery();
        result = rowsAffected > 0 ? "Category deleted successfully" : "No rows affected. Save failed.";
      }

      return result;
    }
  }
}