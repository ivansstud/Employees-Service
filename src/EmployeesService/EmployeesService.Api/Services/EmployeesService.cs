using EmployeesService.Api.Data.Entities;
using EmployeesService.Api.Data.Repositories;
using EmployeesService.Api.Extensions;
using EmployeesService.Api.Models.Common;
using EmployeesService.Api.Models.Requests;
using EmployeesService.Api.Models.Responses;

namespace EmployeesService.Api.Services;

public interface IEmployeesService
{
	Task<Result<IEnumerable<EmployeeResponse>>> GetByCompany(int companyId, CancellationToken cancellationToken);
	Task<Result<IEnumerable<EmployeeResponse>>> GetByDepartment(int departmentId, CancellationToken cancellationToken);
	Task<Result<int>> Create(CreateEmployeeRequest request, CancellationToken cancellationToken);
	Task<Result<int>> Update(UpdateEmployeeRequest request, CancellationToken cancellationToken);
	Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
}

public class EmployeesService(
	IUnitOfWork unitOfWork,
	ILogger<EmployeesService> logger) : IEmployeesService
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly ILogger<EmployeesService> _logger = logger;

	public async Task<Result<IEnumerable<EmployeeResponse>>> GetByDepartment(int departmentId, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Начато получение сотрудников по отделу с id = {DepartmentId}", departmentId);

		try
		{
			IEnumerable<Employee> employees = await _unitOfWork.Employees.GetByDepartmentWithDepartmentAsync(departmentId, cancellationToken);
			IEnumerable<EmployeeResponse> employeesResponse = employees.Select(x => x.ToResponse());

			_logger.LogInformation("Сотрудники по отделу с id = {DepartmentId} успешно получены", departmentId);
			return Result.Success(employeesResponse);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при получении сотрудников по отделу с id = {DepartmentId}", departmentId);
			return Result.Failure<IEnumerable<EmployeeResponse>>("При получении записей произошла ошибка");
		}
	}

	public async Task<Result<IEnumerable<EmployeeResponse>>> GetByCompany(int companyId, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Начато получение сотрудников по компании с id = {CompanyId}", companyId);

		try
		{
			IEnumerable<Employee> employees = await _unitOfWork.Employees.GetByCompanyWithDepartmentAsync(companyId, cancellationToken);
			IEnumerable<EmployeeResponse> employeesSersponse = employees.Select(x => x.ToResponse());

			_logger.LogInformation("Сотрудники по компании с id = {CompanyId} успешно получены", companyId);
			return Result.Success(employeesSersponse);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при получении сотрудников по компании с id = {CompanyId}", companyId);
			return Result.Failure<IEnumerable<EmployeeResponse>>("При получении записей произошла ошибка");
		}
	}

	public async Task<Result<int>> Create(CreateEmployeeRequest request, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Начато создание сотрудника");

		try
		{
			bool isCompanyWithDepartmentExists = await _unitOfWork.Companies
				.ExistsWithDepartmentAsync(request.CompanyId, request.DepartmentId, cancellationToken);

			if (!isCompanyWithDepartmentExists)
			{
				_logger.LogWarning("Компания с отделом не найдена: {CompanyId}, {DepartmentId}",
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
			_logger.LogInformation("Сотрудник успешно создан: {EmployeeId}", employeeId);

			return Result.Success(employeeId);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при создании сотрудника");
			return Result.Failure<int>("При создании записи произошла ошибка");
		}
	}

	public async Task<Result<int>> Update(UpdateEmployeeRequest request, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Начато обновление сотрудника");

		try
		{
			if (request.AllPropertiesAreNull(nameof(request.Id)))
			{
				return Result.Failure<int>("Ничего не выбрано для обновления");
			}

			if (request.DepartmentId is not null)
			{
				int? companyId = request.CompanyId;

				companyId ??= await _unitOfWork.Employees.GetCompanyId(request.Id, cancellationToken);

				bool isCompanyWithDepartmentExists = await _unitOfWork.Companies
					.ExistsWithDepartmentAsync(companyId.Value, request.DepartmentId.Value, cancellationToken);

				if (!isCompanyWithDepartmentExists)
				{
					_logger.LogWarning("Компания CompanyId = {CompanyId} с отделом  DepartmentId = {DepartmentId} не найдена",
						request.CompanyId, request.DepartmentId);
					return Result.Failure<int>("Не удалось найти выбранный отдел");
				}
			}

			int rowsUpdated = await _unitOfWork.Employees.Update(request, cancellationToken);

			await _unitOfWork.CommitAsync(cancellationToken);
			_logger.LogInformation("Выполнено обновление сотрудника");

			return Result.Success(rowsUpdated);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при обновлении сотрудника");
			return Result.Failure<int>("При обновлении записи произошла ошибка");
		}
	}

	public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Начато удаление сотрудника c id = {EmployeeId}", id);

		try
		{
			await _unitOfWork.Employees.DeleteAsync(id, cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);

			_logger.LogInformation("Сотрудник c id = {EmployeeId} успешно удалён", id);

			return Result.Success();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при удалении сотрудника c id = {EmployeeId}", id);
			return Result.Failure("При удалении записи произошла ошибка");
		}
	}
}
