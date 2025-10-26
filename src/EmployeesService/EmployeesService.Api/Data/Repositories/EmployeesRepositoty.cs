using Dapper;
using EmployeesService.Api.Models.Requests;

namespace EmployeesService.Api.Data.Repositories;

public interface IEmployeesRepositoty
{
	Task<int> CreateAsync(CreateEmployeeRequest createModel, CancellationToken cancellationToken);
	Task<bool> ExistsAsync(int departmentId, string passportNumber, CancellationToken cancellationToken);
	Task DeleteAsync(int id, CancellationToken cancellationToken);
}

public class EmployeesRepositoty(IUnitOfWork unitOfWork) : IEmployeesRepositoty
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	public async Task<int> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.Connection.ExecuteScalarAsync<int>(new CommandDefinition(
			commandText: @"
				INSERT INTO employees 
					(name, surname, phone, company_id, department_id, passport_type, passport_number)
				VALUES
					(@Name, @Surname, @Phone, @CompanyId, @DepartmentId, @PassportType, @PassportNumber)
				RETURNING id;",
			parameters: new
			{
				request.Name,
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
