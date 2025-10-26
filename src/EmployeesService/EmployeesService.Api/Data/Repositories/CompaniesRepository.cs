using Dapper;

namespace EmployeesService.Api.Data.Repositories;

public interface ICompaniesRepository
{
	Task<bool> ExistsWithDepartmentAsync(int companyId, int departmentId, CancellationToken cancellationToken);
}

public class CompaniesRepository(IUnitOfWork unitOfWork) : ICompaniesRepository
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	public async Task<bool> ExistsWithDepartmentAsync(int companyId, int departmentId, CancellationToken cancellationToken)
	{
		return await _unitOfWork.Connection.ExecuteScalarAsync<bool>(new CommandDefinition(
			commandText: @"
				SELECT EXISTS(
					SELECT 1 
					FROM departments 
					WHERE id = @DepartmentId 
					AND company_id = @CompanyId
				);",
			parameters: new
			{
				CompanyId = companyId,
				DepartmentId = departmentId
			},
			transaction: _unitOfWork.Transaction,
			cancellationToken: cancellationToken
		));
	}
}