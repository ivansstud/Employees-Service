using System.Reflection;

namespace EmployeesService.Api.Extensions;

public static class ObjectExtensions
{
	public static bool AllPropertiesAreNull<T>(this T obj, params string[] excludedPropertyNames)
	{
		if (obj == null)
		{
			return true;
		}

		var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

		var filteredProperties = properties.Where(prop => !excludedPropertyNames.Contains(prop.Name));

		return filteredProperties.All(prop => prop.GetValue(obj) == null);
	}
}
