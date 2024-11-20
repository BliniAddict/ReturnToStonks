using Microsoft.Data.Sqlite;
using System.IO;

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

      var command = _connection.CreateCommand();
      command.CommandText = "SELECT name, symbol FROM CATEGORIES";
      using (var reader = command.ExecuteReader())
      {
        while (reader.Read())
        {
          throw new NotImplementedException();
        }
      }

      return res;
    }

    public Category GetCategory(string name)
    {
      Category res = new("");

      var command = _connection.CreateCommand();
      command.CommandText = $"SELECT name, symbol FROM CATEGORIES WHERE name = '{name}'";
      using (var reader = command.ExecuteReader())
      {
        while (reader.Read())
        {
          throw new NotImplementedException();
        }
      }
      return res;
    }
  }
}