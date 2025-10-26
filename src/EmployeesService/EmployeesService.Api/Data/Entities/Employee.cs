namespace EmployeesService.Api.Data.Entities;

public class Employee
{
	public int Id { get; set; }
	public required string FirstName { get; set; }
	public required string Surname { get; set; }
	public required string Phone { get; set; }
	public int CompanyId { get; set; }
	public int DepartmentId { get; set; }
	public required string PassportType { get; set; }
	public required string PassportNumber { get; set; }
	public Department Department { get; set; } = null!;
}