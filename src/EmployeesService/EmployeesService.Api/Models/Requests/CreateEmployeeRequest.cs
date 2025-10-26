namespace EmployeesService.Api.Models.Requests;

public class CreateEmployeeRequest
{
	public required string FirstName { get; set; }
	public required string Surname { get; set; }
	public required string Phone { get; set; }
	public int CompanyId { get; set; }
	public int DepartmentId { get; set; }
	public required string PassportType { get; set; }
	public required string PassportNumber { get; set; }
}