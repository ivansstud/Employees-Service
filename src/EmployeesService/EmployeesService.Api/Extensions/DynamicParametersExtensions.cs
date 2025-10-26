using Dapper;

namespace EmployeesService.Api.Extensions;

public static class DynamicParametersExtensions
{
	public static void AddUpdateParameterIfNotNull<T>(
		this DynamicParameters parameters,
		List<string> updates,
		string columnName,
		string paramName,
		T? value) where T : struct
	{
		if (value.HasValue)
		{
			updates.Add($"{columnName} = @{paramName}");
			parameters.Add(paramName, value.Value);
		}
	}

	public static void AddUpdateParameterIfNotNull(
		this DynamicParameters parameters,
		List<string> updates,
		string columnName,
		string paramName,
		string? value)
	{
		if (value != null)
		{
			updates.Add($"{columnName} = @{paramName}");
			parameters.Add(paramName, value);
		}
	}
}

