namespace EmployeesService.Api.Data.Entities;

public class Company
{
	public int Id { get; set; }
	public int DepartmentId { get; set; }
	public required string Name { get; set; }
}