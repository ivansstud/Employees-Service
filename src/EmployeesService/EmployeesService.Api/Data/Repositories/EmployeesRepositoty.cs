using Dapper;
using EmployeesService.Api.Data.Entities;
using EmployeesService.Api.Extensions;
using EmployeesService.Api.Models.Requests;

namespace EmployeesService.Api.Data.Repositories;

public interface IEmployeesRepositoty
{
	Task<IEnumerable<Employee>> GetByCompanyWithDepartmentAsync(int companyId, CancellationToken cancellationToken);
	Task<IEnumerable<Employee>> GetByDepartmentWithDepartmentAsync(int departmentId, CancellationToken cancellationToken);
	Task<int> GetCompanyId(int employeeId, CancellationToken cancellationToken);
	Task<int> CreateAsync(CreateEmployeeRequest createModel, CancellationToken cancellationToken);
	Task<int> Update(UpdateEmployeeRequest request, CancellationToken cancellationToken);
	Task<bool> ExistsAsync(int departmentId, string passportNumber, CancellationToken cancellationToken);
	Task DeleteAsync(int id, CancellationToken cancellationToken);
}

public class EmployeesRepositoty(IUnitOfWork unitOfWork) : IEmployeesRepositoty
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	public async Task<IEnumerable<Employee>> GetByCompanyWithDepartmentAsync(int companyId, CancellationToken cancellationToken)
	{
		return await _unitOfWork.Connection.QueryAsync<Employee, Department, Employee>(
			new CommandDefinition(
				commandText: @"
					SELECT 
						emp.id AS Id,
						emp.first_name AS FirstName,
						emp.surname AS Surname,
						emp.phone AS Phone,
						emp.company_id AS CompanyId,
						emp.department_id AS DepartmentId,
						emp.passport_type AS PassportType,
						emp.passport_number AS PassportNumber,
						dep.name AS Name,
						dep.phone AS Phone
					FROM employees AS emp
					JOIN departments AS dep ON dep.id = emp.department_id
					WHERE emp.company_id = @CompanyId;",
				parameters: new { CompanyId = companyId },
				transaction: _unitOfWork.Transaction,
				cancellationToken: cancellationToken
			),
			map: (emp, dep) =>
			{
				emp.Department = dep;
				return emp;
			},
			splitOn: "Name"
		);
	}

	public async Task<int> GetCompanyId(int employeeId, CancellationToken cancellationToken)
	{
		return await _unitOfWork.Connection.ExecuteScalarAsync<int>(new CommandDefinition(
			commandText: @"
				SELECT company_id
				FROM employees
				WHERE id = @EmployeeId;",
			parameters: new { EmployeeId = employeeId },
			transaction: _unitOfWork.Transaction,
			cancellationToken: cancellationToken
		));
	}

	public async Task<IEnumerable<Employee>> GetByDepartmentWithDepartmentAsync(int departmentId, CancellationToken cancellationToken)
	{
		return await _unitOfWork.Connection.QueryAsync<Employee, Department, Employee>(
			new CommandDefinition(
				commandText: @"
					SELECT 
						emp.id AS Id,
						emp.first_name AS FirstName,
						emp.surname AS Surname,
						emp.phone AS Phone,
						emp.company_id AS CompanyId,
						emp.department_id AS DepartmentId,
						emp.passport_type AS PassportType,
						emp.passport_number AS PassportNumber,
						dep.name AS Name,
						dep.phone AS Phone
					FROM employees AS emp
					JOIN departments AS dep ON dep.id = emp.department_id
					WHERE emp.department_id = @DepartmentId;",
				parameters: new { DepartmentId = departmentId },
				transaction: _unitOfWork.Transaction,
				cancellationToken: cancellationToken
			),
			map: (emp, dep) =>
			{
				emp.Department = dep;
				return emp;
			},
			splitOn: "Name"
		);
	}

	public async Task<int> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.Connection.ExecuteScalarAsync<int>(new CommandDefinition(
			commandText: @"
				INSERT INTO employees 
					(first_name, surname, phone, company_id, department_id, passport_type, passport_number)
				VALUES
					(@FirstName, @Surname, @Phone, @CompanyId, @DepartmentId, @PassportType, @PassportNumber)
				RETURNING id;",
			parameters: new
			{
				request.FirstName,
				request.Surname,
				request.Phone,
				request.CompanyId,
				request.DepartmentId,
				request.PassportType,
				request.PassportNumber
			},
			transaction: _unitOfWork.Transaction,
			cancellationToken: cancellationToken
		));
	}

	public async Task<int> Update(UpdateEmployeeRequest request, CancellationToken cancellationToken)
	{
		List<string> updates = [];
		DynamicParameters parameters = new();

		parameters.Add("Id", request.Id);
		parameters.AddUpdateParameterIfNotNull(updates, "first_name", "FirstName", request.FirstName);
		parameters.AddUpdateParameterIfNotNull(updates, "surname", "Surname", request.Surname);
		parameters.AddUpdateParameterIfNotNull(updates, "phone", "Phone", request.Phone);
		parameters.AddUpdateParameterIfNotNull(updates, "company_id", "CompanyId", request.CompanyId);
		parameters.AddUpdateParameterIfNotNull(updates, "department_id", "DepartmentId", request.DepartmentId);
		parameters.AddUpdateParameterIfNotNull(updates, "passport_type", "PassportType", request.PassportType);
		parameters.AddUpdateParameterIfNotNull(updates, "passport_number", "PassportNumber", request.PassportNumber);

		return await _unitOfWork.Connection.ExecuteAsync(new CommandDefinition(
			commandText: $"UPDATE employees SET {string.Join(", ", updates)} WHERE id = @Id;",
			parameters: parameters,
			transaction: _unitOfWork.Transaction,
			cancellationToken: cancellationToken
		));
	}

	public async Task<bool> ExistsAsync(int departmentId, string passportNumber, CancellationToken cancellationToken)
	{
		return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(new CommandDefinition(
			commandText: @"
				SELECT EXISTS (
					SELECT 1
					FROM employees
					WHERE department_id = @DepartmentId
					  AND passport_number = @PassportNumber
				);",
			parameters: new
			{
				PassportNumber = passportNumber,
				DepartmentId = departmentId
			},
			transaction: _unitOfWork.Transaction,
			cancellationToken: cancellationToken
		));
	}

	public async Task DeleteAsync(int id, CancellationToken cancellationToken)
	{
		await _unitOfWork.Connection.ExecuteAsync(new CommandDefinition(
			commandText: @"
				DELETE FROM employees
				WHERE id = @Id;
			",
			parameters: new { Id = id },
			transaction: _unitOfWork.Transaction,
			cancellationToken: cancellationToken
		));
	}
}
