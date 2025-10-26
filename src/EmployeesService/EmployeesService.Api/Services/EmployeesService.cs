using EmployeesService.Api.Data;
using EmployeesService.Api.Models.Common;
using EmployeesService.Api.Models.Requests;

namespace EmployeesService.Api.Services;

public interface IEmployeesService
{
	Task<Result<int>> Create(CreateEmployeeRequest request, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
}

public class EmployeesService(
	IUnitOfWork unitOfWork,
	ILogger<EmployeesService> logger) : IEmployeesService
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly ILogger<EmployeesService> _logger = logger;

	public async Task<Result<int>> Create(CreateEmployeeRequest request, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Начало создания сотрудника");

		try
		{
			bool isCompanyWithDepartmentExists = await _unitOfWork.Companies
				.ExistsWithDepartmentAsync(request.CompanyId, request.DepartmentId, cancellationToken);

			if (!isCompanyWithDepartmentExists)
			{
				_logger.LogWarning("Компания с отделом не найдена: CompanyId={CompanyId}, DepartmentId={DepartmentId}",
					request.CompanyId, request.DepartmentId);
				return Result.Failure<int>("Не удалось найти выбранный отдел");
			}

			bool isExists = await _unitOfWork.Employees
				.ExistsAsync(request.DepartmentId, request.PassportNumber, cancellationToken);

			if (isExists)
			{
				return Result.Failure<int>("Данный сотрудник уже существует в выбранном отделе");
			}

			int employeeId = await _unitOfWork.Employees.CreateAsync(request, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);

			_logger.LogInformation("Сотрудник успешно создан: EmployeeId={EmployeeId}", employeeId);

			return Result.Success(employeeId);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при создании сотрудника");
			return Result.Failure<int>("При создании записи произошла ошибка");
		}
	}

	public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Начато удаление сотрудника");

		try
		{
			await _unitOfWork.Employees.DeleteAsync(id, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);

			_logger.LogInformation("Сотрудник успешно удалён");

			return Result.Success();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при удалении сотрудника");
			return Result.Failure("При удалении записи произошла ошибка");
		}
	}
}
