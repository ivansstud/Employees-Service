namespace EmployeesService.Api.Data.Entities;

public class Employee
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public required string Surname { get; set; }
	public required string Phone { get; set; }
	public int CompanyId { get; set; }
	public int DepartmentId { get; set; }
	public required string PassportType { get; set; }
	public required string PassportNumber { get; set; }
}