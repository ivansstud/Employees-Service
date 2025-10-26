using EmployeesService.Api.Data.Entities;
using EmployeesService.Api.Models.Responses;

namespace EmployeesService.Api.Extensions;

public static class EmployeeExtensions
{
	public static EmployeeResponse ToResponse(this Employee employee)
	{
		return new EmployeeResponse
		{
			Id = employee.Id,
			Name = employee.FirstName,
			Surname = employee.Surname,
			Phone = employee.Phone,
			CompanyId = employee.CompanyId,
			Passport = new PassportResponse
			{
				Number = employee.PassportNumber,
				Type = employee.PassportType
			},
			Department = new DepartmentResponse
			{
				Name = employee.Department.Name,
				Phone = employee.Department.Phone
			}
		};
	}
}
