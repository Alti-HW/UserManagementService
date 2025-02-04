#nullable disable

using System.Reflection;

namespace UserManagement.Application.Extensions;

public static class ObjectExtensions
{
    public static Dictionary<string, string> ToFilteredDictionary<T>(this T obj)
    {
        if (obj == null)
        {
            return new Dictionary<string, string>();
        }

        return obj.GetType()
                  .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                  .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj)?.ToString())
                  .Where(dict => !string.IsNullOrWhiteSpace(dict.Value))
                  .ToDictionary(dict => dict.Key, dict => dict.Value.ToString());
    }
}
