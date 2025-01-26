using System.Collections;
using System.Reflection;

namespace ReturnToStonks
{
  public class Utilities
  {
    public static DateTime ThisMonth = new(DateTime.Now.Year, DateTime.Now.Month, 1);
    public static bool ArePropertiesEqual<T>(T obj1, T obj2, List<string> ignoredProperties = null)
    {
      if (obj1 == null ^ obj2 == null)
        return false;
      if ((obj1 == null && obj2 == null) || ReferenceEquals(obj1, obj2))
        return true;

      Type type = obj1.GetType();
      PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
      foreach (var property in properties)
      {
        object value1 = property.GetValue(obj1);
        object value2 = property.GetValue(obj2);

        if (ignoredProperties != null && ignoredProperties.Any(prop => prop.ToLower() == property.Name.ToLower()))
          continue;

        if (value1 == null ^ value2 == null)
          return false;
        else
        {
          if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
            continue;
          if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string) || !property.PropertyType.IsClass)
          {
            if (!Equals(value1, value2))
              return false;
          }
          else
          {
            if (!ArePropertiesEqual(value1, value2))
              return false;
          }
        }
      }
      return true;
    }
    public static DateTime CalculateNextDueDate(DateTime baseDate, Recurrence recurrence)
    {
      if (recurrence == null) return DateTime.MinValue;

      return recurrence.Unit switch
      {
        "day" => baseDate.AddDays(recurrence.Span),
        "week" => baseDate.AddDays(7 * recurrence.Span),
        "month" => baseDate.AddMonths(recurrence.Span),
        "year" => baseDate.AddYears(recurrence.Span),
        _ => baseDate
      };
    }
  }
}
