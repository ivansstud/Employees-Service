namespace EmployeesService.Api.Models.Responses;

public class EmployeeResponse
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public required string Surname { get; set; }
	public required string Phone { get; set; }
	public int CompanyId { get; set; }
	public required PassportResponse Passport { get; set; }
	public required DepartmentResponse Department { get; set; }
	
}

public class PassportResponse
{
	public required string Type { get; set; }
	public required string Number { get; set; }
}

public class DepartmentResponse
{
	public required string Name { get; set; }
	public required string Phone { get; set; }
}
