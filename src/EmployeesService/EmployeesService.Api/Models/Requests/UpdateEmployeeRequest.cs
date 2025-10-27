using System.Text.Json.Serialization;

namespace EmployeesService.Api.Models.Requests;

public record UpdateEmployeeRequest(
	string? Name,
	string? Surname,
	string? Phone,
	int? DepartmentId,
	int? CompanyId,
	string? PassportType,
	string? PassportNumber)
{
	[JsonIgnore]
	public int Id { get; set; }
}
