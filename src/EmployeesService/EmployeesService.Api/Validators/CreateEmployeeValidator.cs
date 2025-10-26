using EmployeesService.Api.Models.Requests;
using FluentValidation;

namespace EmployeesService.Api.Validators;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeRequest>
{
	public CreateEmployeeValidator()
	{
		RuleFor(x => x.FirstName)
			.NotEmpty().WithMessage("Имя обязательно для заполнения")
			.MaximumLength(100).WithMessage("Имя не может превышать 100 символов");

		RuleFor(x => x.Surname)
			.NotEmpty().WithMessage("Фамилия обязательна для заполнения")
			.MaximumLength(100).WithMessage("Фамилия не может превышать 100 символов");

		RuleFor(x => x.Phone)
			.NotEmpty().WithMessage("Телефон обязателен для заполнения")
			.MaximumLength(50).WithMessage("Телефон не может превышать 50 символов")
			.Matches(@"^\+?[0-9\s\-\(\)]+$").WithMessage("Некорректный формат телефона");

		RuleFor(x => x.CompanyId)
			.GreaterThan(0).WithMessage("CompanyId должен быть положительным числом");

		RuleFor(x => x.DepartmentId)
			.GreaterThan(0).WithMessage("DepartmentId должен быть положительным числом");

		RuleFor(x => x.PassportType)
			.NotEmpty().WithMessage("Тип паспорта обязателен для заполнения")
			.MaximumLength(50).WithMessage("Тип паспорта не может превышать 50 символов");

		RuleFor(x => x.PassportNumber)
			.NotEmpty().WithMessage("Номер паспорта обязателен для заполнения")
			.Length(10).WithMessage("Номер паспорта должен состоять из 10 цифр")
			.Matches(@"^\d{10}$").WithMessage("Номер паспорта должен содержать только цифры");
	}
}
