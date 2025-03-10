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

    public static T GetPropertyValue<T>(this object obj, string propertyName)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj), "Object cannot be null.");

        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentException("Property name cannot be null or empty.", nameof(propertyName));

        try
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            
            if (propertyInfo == null)
                throw new ArgumentException($"Property '{propertyName}' not found on type '{obj.GetType().FullName}'.");

            return (T)propertyInfo.GetValue(obj);
        }
        catch (TargetInvocationException tie)
        {
            throw new InvalidOperationException("Error accessing property value.", tie);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving the property '{propertyName}' value.", ex);
        }
    }
}
