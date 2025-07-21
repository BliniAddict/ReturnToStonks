using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Transactions;
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

        #region HelperMethods
        private static SqliteParameter CreateParameter(string name, object? value)
        {
            if (value is DateTime? || value is DateTime)
            {
                DateTime? date = (DateTime?)value;
                if (date.HasValue)
                    return new SqliteParameter(name, date.Value.ToString("yyyy-MM-dd"));
            }

            return new SqliteParameter(name, value ?? DBNull.Value);
        }

        private static List<string> BuildEqualsConditions<T>(T row)
        {
            List<string> conditions = new();

            PropertyInfo[] properties = row.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties.Where(p => p.PropertyType == typeof(string) || !p.PropertyType.IsClass))
            {
                object? value = property.GetValue(row);
                string columnName = property.Name.ToLower();

                string condition = value switch
                {
                    null => $"{columnName} IS NULL",
                    string => $"{columnName}='{value}'",
                    DateTime => $"{columnName}='{((DateTime)value):yyyy-MM-dd}'",
                    double => $"{columnName}={((double)value).ToString("0.0", CultureInfo.InvariantCulture)}",
                    _ => $"{columnName}={value}"
                };
                if (condition != null)
                    conditions.Add(condition);
            }
            return conditions;
        }

        private static string BuildFutureDateEqualsConditions<T>(T classType)
        {
            List<string> dateConditions = new();

            //get date-relevant Properties
            Type type = classType.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var recurrenceProperty = properties.FirstOrDefault(name => name.Name == "Recurrence");
            Recurrence? recurrence = (Recurrence?)recurrenceProperty?.GetValue(classType);

            var dateProperty = properties.FirstOrDefault(name => name.Name == "Date");
            DateTime? date = (DateTime?)dateProperty?.GetValue(classType);

            if (recurrence != null && date != null)
            {
                DateTime nextDate = Utilities.CalculateNextDueDate((DateTime)date, recurrence);
                while (nextDate <= Utilities.In2Months)
                {
                    dateConditions.Add($"date='{nextDate:yyyy-MM-dd}'");
                    nextDate = Utilities.CalculateNextDueDate(nextDate, recurrence);
                }
            }

            if (dateConditions.Count > 0)
                return " OR " + string.Join(" OR ", dateConditions);
            else
                return string.Empty;
        }
        #endregion

        #region Transactions
        public string SaveTransaction(Transaction selectedTransaction, Transaction? oldTransaction = null)
        {
            int rowsAffected = 0;

            using (var command = _connection.CreateCommand())
            {
                if (oldTransaction != null)
                {
                    if (oldTransaction.IsRecurring)
                        rowsAffected += ChangeRecurringTransactions(selectedTransaction, oldTransaction);
                    rowsAffected = ChangeTransaction(selectedTransaction, oldTransaction);
                }
                else
                {
                    if (!selectedTransaction.IsRecurring)
                        selectedTransaction.Recurrence = null;

                    command.CommandText = "INSERT INTO Transactions (purpose, category, amount, date, recurrence_span, recurrence_unit) " +
                    "VALUES (@newPurpose, @newCategory, @newAmount, @newDate, @newRecurrence_Span, @newRecurrence_Unit)";

                    command.Parameters.Add(CreateParameter("@newPurpose", selectedTransaction.Purpose));
                    command.Parameters.Add(CreateParameter("@newCategory", selectedTransaction.Category?.Name));
                    command.Parameters.Add(CreateParameter("@newAmount", selectedTransaction.Amount));
                    command.Parameters.Add(CreateParameter("@newDate", selectedTransaction.Date));
                    command.Parameters.Add(CreateParameter("@newRecurrence_Span", selectedTransaction.Recurrence?.Span ?? 0));
                    command.Parameters.Add(CreateParameter("@newRecurrence_Unit", selectedTransaction.Recurrence?.Unit));

                    rowsAffected = command.ExecuteNonQuery();
                }
            }
            return rowsAffected > 0 ? "Transaction saved successfully" : "No rows affected. Save failed.";
        }
        public List<Transaction> GetTransactions()
        {
            List<Transaction> res = new();

            using var command = _connection.CreateCommand();
            command.CommandText = $"SELECT purpose, category, amount, date, recurrence_span, recurrence_unit FROM Transactions ";

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
            int rowsAffected = 0;

            List<string> conditions = BuildTransactionEqualsConditions(selectedTransaction);
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Transactions WHERE " + string.Join(" AND ", conditions);

                if (selectedTransaction.IsRecurring)
                    command.CommandText += BuildFutureDateEqualsConditions(selectedTransaction);

                rowsAffected += command.ExecuteNonQuery();

                if (selectedTransaction.IsRecurring)
                    ChangeRecurringTransactions(new Transaction(selectedTransaction) { IsRecurring = false }, selectedTransaction);
            }

            if (rowsAffected == 1)
                return "1 Transaction deleted successfully.";
            else if (rowsAffected > 0)
                return $"{rowsAffected} Transactions deleted successfully.";
            else
                return "No rows affected. Delete failed.";
        }

        private int ChangeTransaction(Transaction selectedTransaction, Transaction oldTransaction)
        {
            int rowsAffected = 0;

            using (var command = _connection.CreateCommand())
            {
                List<string> setConditions = BuildTransactionEqualsConditions(selectedTransaction);
                List<string> whereConditions = BuildTransactionEqualsConditions(oldTransaction);

                command.CommandText = $"UPDATE Transactions " +
                  $"SET {string.Join(", ", setConditions)} ".Replace(" IS ", "=") +
                  $"WHERE {string.Join(" AND ", whereConditions)}";

                return command.ExecuteNonQuery();
            }
        }
        private int ChangeRecurringTransactions(Transaction transaction, Transaction oldTransaction)
        {
            int rowsAffected = 0;
            List<string> propsToIgnore = new() { nameof(transaction.Date), nameof(transaction.IsPayed) };

            List<Transaction> affectedRows = GetTransactions().Where(a =>
                  a.Date != oldTransaction.Date &&
                  Utilities.ArePropertiesEqual(a, oldTransaction, propsToIgnore)).OrderBy(date => date.Date).ToList();

            foreach (Transaction affectedTransaction in affectedRows)
            {
                Transaction changedTransaction = new(transaction) { Date = affectedTransaction.Date };
                rowsAffected += ChangeTransaction(changedTransaction, affectedTransaction);
            }
            return rowsAffected;
        }
        private static List<string> BuildTransactionEqualsConditions(Transaction transaction)
        {
            List<string> conditions = BuildEqualsConditions(transaction);
            string dateCondition = conditions.First(c => c.Contains("date"));
            conditions.RemoveAll(c => c.Contains("isrecurring") | c.Contains("ispayed") | c.Contains("date"));

            //add conditions associated with class properties
            conditions.Add(transaction.Category == null
                ? "category IS NULL"
                : $"category='{transaction.Category.Name}'");

            if (!transaction.IsRecurring || transaction.Recurrence == null)
            {
                conditions.Add("recurrence_unit IS NULL");
                conditions.Add("recurrence_span=0");
            }
            else
            {
                conditions.Add($"recurrence_unit='{transaction.Recurrence.Unit}'");
                conditions.Add($"recurrence_span={transaction.Recurrence.Span}");
            }

            conditions.Add(dateCondition);
            return conditions;
        }
        #endregion

        #region Debts
        public string SaveDebt(Debt selectedDebt, Debt? oldDebt)
        {
            int rowsAffected = 0;

            using (var command = _connection.CreateCommand())
            {
                if (oldDebt != null)
                {
                    List<string> setConditions = BuildDebtEqualsConditions(selectedDebt);
                    List<string> whereConditions = BuildDebtEqualsConditions(oldDebt);

                    command.CommandText = $"UPDATE Debts " +
                      $"SET {string.Join(", ", setConditions)} ".Replace(" IS ", "=") +
                      $"WHERE {string.Join(" AND ", whereConditions)}";

                    rowsAffected = command.ExecuteNonQuery();
                }
                else
                {
                    command.CommandText = "INSERT INTO Debts (purpose, name, category, amount, due_date) " +
                    "VALUES (@newPurpose, @newName, @newCategory, @newAmount, @newDue_date)";

                    command.Parameters.Add(CreateParameter("@newPurpose", selectedDebt.Purpose));
                    command.Parameters.Add(CreateParameter("@newName", selectedDebt.Person?.Name));
                    command.Parameters.Add(CreateParameter("@newCategory", selectedDebt.Category?.Name));
                    command.Parameters.Add(CreateParameter("@newAmount", selectedDebt.Amount));
                    command.Parameters.Add(CreateParameter("@newDue_date", selectedDebt.Due_date));

                    rowsAffected = command.ExecuteNonQuery();
                }
            }
            return rowsAffected > 0 ? "Debt saved successfully" : "No rows affected. Save failed.";
        }
        public List<Debt> GetDebts()
        {
            List<Debt> res = new();
            List<Debt> addedLater = new();

            using var command = _connection.CreateCommand();
            command.CommandText = $"SELECT purpose, name, category, amount, due_date FROM Debts ORDER BY due_date";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Debt de = new(
                      reader.GetString(0),
                      reader.IsDBNull(1) ? null : GetPerson(reader.GetString(1)),
                      reader.IsDBNull(2) ? null : GetCategory(reader.GetString(2)),
                      reader.GetDouble(3),
                      reader.IsDBNull(4) ? null : DateTime.ParseExact(reader.GetString(4), "yyyy-MM-dd", CultureInfo.InvariantCulture));

                    if (de.Due_date != null)
                        res.Add(de);
                    else
                        addedLater.Add(de);
                }
            }

            res.AddRange(addedLater);

            return res;
        }
        public string DeleteDebt(Debt selectedDebt)
        {
            int rowsAffected = 0;

            List<string> conditions = BuildDebtEqualsConditions(selectedDebt);
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Debts WHERE " + string.Join(" AND ", conditions);
                rowsAffected += command.ExecuteNonQuery();
            }


            if (rowsAffected == 1)
                return "Debt deleted successfully.";
            else
                return "No rows affected. Delete failed.";
        }

        private static List<string> BuildDebtEqualsConditions(Debt debt)
        {
            List<string> conditions = BuildEqualsConditions(debt);

            //add conditions associated with class properties
            conditions.Add(debt.Category == null
                ? "category IS NULL"
                : $"category='{debt.Category.Name}'");

            return conditions;
        }
        #endregion

        #region Categories
        public string SaveCategory(Category selectedCategory, Category? oldCategory = null)
        {
            string result = string.Empty;

            using (var command = _connection.CreateCommand())
            {
                if (oldCategory == null) //INSERT
                {
                    command.CommandText = "INSERT INTO Categories (name, symbol) VALUES (@newName, @newSymbol)";
                    command.Parameters.Add(CreateParameter("@newName", selectedCategory.Name));
                    command.Parameters.Add(CreateParameter("@newSymbol", selectedCategory.Symbol));
                }
                else //UPDATE
                {
                    List<string> setConditions = BuildEqualsConditions(selectedCategory);
                    List<string> whereConditions = BuildEqualsConditions(oldCategory);
                    command.CommandText = $"UPDATE Categories SET {string.Join(", ", setConditions)} WHERE {string.Join(" AND ", whereConditions)}";
                }

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
        private Category GetCategory(string name)
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
            int rowsAffected = 0;

            List<string> conditions = BuildEqualsConditions(selectedCategory);
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Categories WHERE " + string.Join(" AND ", conditions);
                rowsAffected += command.ExecuteNonQuery();
            }

            if (rowsAffected == 1)
                return "Category deleted successfully.";
            else
                return "No rows affected. Delete failed.";
        }
        #endregion

        #region Persons
        public string SavePerson(Person selectedPerson, Person? oldPerson = null)
        {
            string result = string.Empty;

            using (var command = _connection.CreateCommand())
            {
                if (oldPerson == null) //INSERT
                {
                    command.CommandText = "INSERT INTO Persons (name, contact_method, contact_id) " +
                                          "VALUES (@newName, @newContact_method, @newContact_id)";
                    command.Parameters.Add(CreateParameter("@newName", selectedPerson.Name));
                    command.Parameters.Add(CreateParameter("@newContact_method", selectedPerson.Contact_Method));
                    command.Parameters.Add(CreateParameter("@newContact_id", selectedPerson.Contact_ID));
                }
                else //UPDATE
                {
                    List<string> setConditions = BuildEqualsConditions(selectedPerson);
                    List<string> whereConditions = BuildEqualsConditions(oldPerson);
                    command.CommandText = $"UPDATE Persons SET {string.Join(", ", setConditions).Replace(" IS ", "=")} WHERE {string.Join(" AND ", whereConditions)}";
                }

                int rowsAffected = command.ExecuteNonQuery();
                result = rowsAffected > 0 ? "Person saved successfully" : "No rows affected. Save failed.";
            }

            return result;
        }

        private Person GetPerson(string name)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT name, contact_method, contact_id FROM Persons WHERE name = @name";

            command.Parameters.Add(CreateParameter("@name", name));

            using var reader = command.ExecuteReader();

            if (reader.Read())
                return new Person(
                        reader.IsDBNull(0) ? null : reader.GetString(0),
                        reader.IsDBNull(1) ? null : reader.GetString(1),
                        reader.IsDBNull(2) ? null : reader.GetString(2)
                        );

            return null;
        }
        public List<Person> GetPersons()
        {
            List<Person> res = new();

            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT name, contact_method, contact_id FROM Persons";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                    res.Add(new Person(
                        reader.IsDBNull(0) ? null : reader.GetString(0),
                        reader.IsDBNull(1) ? null : reader.GetString(1),
                        reader.IsDBNull(2) ? null : reader.GetString(2)
                        ));
            }

            return res;
        }

        public string DeletePerson(Person selectedPerson)
        {
            int rowsAffected = 0;

            List<string> conditions = BuildEqualsConditions(selectedPerson);
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Persons WHERE " + string.Join(" AND ", conditions);
                rowsAffected += command.ExecuteNonQuery();
            }

            if (rowsAffected == 1)
                return "Person deleted successfully.";
            else
                return "No rows affected. Delete failed.";
        }
        #endregion
    }
}