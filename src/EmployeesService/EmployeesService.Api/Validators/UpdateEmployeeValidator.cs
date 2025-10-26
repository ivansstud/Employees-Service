using EmployeesService.Api.Models.Requests;
using FluentValidation;

namespace EmployeesService.Api.Validators;

public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeRequest>
{
	public UpdateEmployeeValidator()
	{
		When(x => x.FirstName != null, () =>
		{
			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("Имя обязательно для заполнения")
				.MaximumLength(100).WithMessage("Имя не может превышать 100 символов");
		});

		When(x => x.Surname != null, () =>
		{
			RuleFor(x => x.Surname)
				.NotEmpty().WithMessage("Фамилия обязательна для заполнения")
				.MaximumLength(100).WithMessage("Фамилия не может превышать 100 символов");
		});

		When(x => x.Phone != null, () =>
		{
			RuleFor(x => x.Phone)
				.NotEmpty().WithMessage("Телефон обязателен для заполнения")
				.MaximumLength(50).WithMessage("Телефон не может превышать 50 символов")
				.Matches(@"^\+?[0-9\s\-\(\)]+$").WithMessage("Некорректный формат телефона");
		});

		When(x => x.CompanyId != null, () =>
		{
			RuleFor(x => x.CompanyId)
				.GreaterThan(0).WithMessage("Неверный формат выбранной компании");
			RuleFor(x => x.DepartmentId)
				.NotNull().WithMessage("Изменение компании должен быть вместе с отделом");
		});

		When(x => x.DepartmentId != null, () =>
		{
			RuleFor(x => x.DepartmentId)
				.GreaterThan(0).WithMessage("DepartmentId должен быть положительным числом");
		});

		When(x => x.PassportType != null, () =>
		{
			RuleFor(x => x.PassportType)
				.NotEmpty().WithMessage("Тип паспорта обязателен для заполнения")
				.MaximumLength(50).WithMessage("Тип паспорта не может превышать 50 символов");
		});

		When(x => x.PassportNumber != null, () =>
		{
			RuleFor(x => x.PassportNumber)
				.NotEmpty().WithMessage("Номер паспорта обязателен для заполнения")
				.Length(10).WithMessage("Номер паспорта должен состоять из 10 цифр")
				.Matches(@"^\d{10}$").WithMessage("Номер паспорта должен содержать только цифры");
		});
	}
}
