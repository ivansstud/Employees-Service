using System.Text.Json.Serialization;

namespace EmployeesService.Api.Models.Requests;

public record UpdateEmployeeRequest(
	string? FirstName,
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
