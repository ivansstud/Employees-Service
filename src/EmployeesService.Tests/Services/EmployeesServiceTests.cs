using EmployeesService.Api.Data.Entities;
using EmployeesService.Api.Data.Repositories;
using EmployeesService.Api.Models.Requests;
using EmployeesService.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeesService.Tests.Services;

public class EmployeesServiceTests
{
	private readonly Mock<IEmployeesRepositoty> _employeesRepoMock;
	private readonly Mock<ICompaniesRepository> _companiesRepoMock;
	private readonly Mock<IUnitOfWork> _unitOfWorkMock;
	private readonly Mock<ILogger<Api.Services.EmployeesService>> _loggerMock;
	private readonly Api.Services.EmployeesService _service;

	public EmployeesServiceTests()
	{
		_employeesRepoMock = new Mock<IEmployeesRepositoty>();
		_companiesRepoMock = new Mock<ICompaniesRepository>();
		var cacheMock = new Mock<IEmployeesCacheService>();
		_unitOfWorkMock = new Mock<IUnitOfWork>();
		_loggerMock = new Mock<ILogger<Api.Services.EmployeesService>>();

		_unitOfWorkMock.Setup(u => u.Employees).Returns(_employeesRepoMock.Object);
		_unitOfWorkMock.Setup(u => u.Companies).Returns(_companiesRepoMock.Object);

		_service = new Api.Services.EmployeesService(_unitOfWorkMock.Object, cacheMock.Object, _loggerMock.Object);
	}

	[Fact]
	public async Task Create_ShouldReturnFailure_WhenCompanyDepartmentNotExists()
	{
		// Arrange
		var request = new CreateEmployeeRequest
		{
			Name = "Ivan",
			Surname = "Petrov",
			Phone = "123",
			CompanyId = 1,
			DepartmentId = 1,
			PassportType = "RF",
			PassportNumber = "999999"
		};
		_companiesRepoMock
			.Setup(r => r.ExistsWithDepartmentAsync(1, 1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		// Act
		var result = await _service.CreateAsync(request, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("Не удалось найти выбранный отдел", result.Error);
	}

	[Fact]
	public async Task Create_ShouldReturnFailure_WhenEmployeeAlreadyExists()
	{
		// Arrange
		var request = new CreateEmployeeRequest
		{
			Name = "Ivan",
			Surname = "Petrov",
			Phone = "123",
			CompanyId = 1,
			DepartmentId = 1,
			PassportType = "RF",
			PassportNumber = "999999"
		};
		_companiesRepoMock
			.Setup(r => r.ExistsWithDepartmentAsync(1, 1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);
		_employeesRepoMock
			.Setup(r => r.ExistsAsync(1, "999999", It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		// Act
		var result = await _service.CreateAsync(request, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("Данный сотрудник уже существует в выбранном отделе", result.Error);
	}

	[Fact]
	public async Task Create_ShouldReturnSuccess_WhenValid()
	{
		// Arrange
		var request = new CreateEmployeeRequest
		{
			Name = "Ivan",
			Surname = "Petrov",
			Phone = "123",
			CompanyId = 1,
			DepartmentId = 1,
			PassportType = "RF",
			PassportNumber = "999999"
		};
		_companiesRepoMock
			.Setup(r => r.ExistsWithDepartmentAsync(1, 1, It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);
		_employeesRepoMock
			.Setup(r => r.ExistsAsync(1, "999999", It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);
		_employeesRepoMock
			.Setup(r => r.CreateAsync(It.IsAny<CreateEmployeeRequest>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(42);

		// Act
		var result = await _service.CreateAsync(request, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(42, result.Value);
		_unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Create_ShouldReturnFailure_OnException()
	{
		// Arrange
		var request = new CreateEmployeeRequest
		{
			Name = "Ivan",
			Surname = "Petrov",
			Phone = "123",
			CompanyId = 1,
			DepartmentId = 1,
			PassportType = "RF",
			PassportNumber = "999999"
		};
		_companiesRepoMock
			.Setup(r => r.ExistsWithDepartmentAsync(1, 1, It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("DB error"));

		// Act
		var result = await _service.CreateAsync(request, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("При создании записи произошла ошибка", result.Error);
	}

	[Fact]
	public async Task Update_ShouldFail_WhenAllPropertiesAreNull()
	{
		// Arrange
		var request = new UpdateEmployeeRequest(null, null, null, null, null, null, null)
		{
			Id = 1
		};

		// Act
		var result = await _service.UpdateAsync(request, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("Ничего не выбрано для обновления", result.Error);
	}

	[Fact]
	public async Task Update_ShouldReturnFailure_WhenDepartmentNotFound()
	{
		// Arrange
		var request = new UpdateEmployeeRequest(null, null, "111", 2, 1, null, null)
		{
			Id = 5
		};
		_employeesRepoMock
			.Setup(r => r.GetCompanyIdAstnc(5, It.IsAny<CancellationToken>()))
			.ReturnsAsync(1);
		_companiesRepoMock
			.Setup(r => r.ExistsWithDepartmentAsync(1, 2, It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		// Act
		var result = await _service.UpdateAsync(request, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("Не удалось найти выбранный отдел", result.Error);
	}

	[Fact]
	public async Task Update_ShouldReturnSuccess_WhenValid()
	{
		// Arrange
		var request = new UpdateEmployeeRequest("Ivan", null, "777", 2, 1, "RF", "111")
		{
			Id = 3
		};
		_companiesRepoMock
			.Setup(r => r.ExistsWithDepartmentAsync(1, 2, It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);
		_employeesRepoMock
			.Setup(r => r.UpdateAsync(request, It.IsAny<CancellationToken>()))
			.ReturnsAsync(1);

		// Act
		var result = await _service.UpdateAsync(request, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		Assert.Equal(1, result.Value);
		_unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_ShouldCommitAndReturnSuccess()
	{
		// Arrange
		_employeesRepoMock
			.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act
		var result = await _service.DeleteAsync(1, CancellationToken.None);

		// Assert
		Assert.True(result.IsSuccess);
		_unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task DeleteAsync_ShouldReturnFailure_OnException()
	{
		// Arrange
		_employeesRepoMock
			.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
			.ThrowsAsync(new Exception("DB error"));

		// Act
		var result = await _service.DeleteAsync(1, CancellationToken.None);

		// Assert
		Assert.True(result.IsFailure);
		Assert.Equal("При удалении записи произошла ошибка", result.Error);
	}
}