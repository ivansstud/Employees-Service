using EmployeesService.Api.Extensions;
using EmployeesService.Api.Models.Requests;
using EmployeesService.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesService.Api.Endpoints;

public class EmployeesEndpoints
{
	public static void MapEndpoints(WebApplication app)
	{
		var group = app.MapGroup("employees");

		group.MapGet("/company/{id}", GetByCompany);

		group.MapGet("/department/{id}", GetByDepartment);

		group.MapPost("/", Create)
			.WithValidationFilter<CreateEmployeeRequest>();

		group.MapPut("/{id}", Update)
			.WithValidationFilter<UpdateEmployeeRequest>();

		group.MapDelete("/{id}", Delete);
	}

	private static async Task<IResult> GetByCompany(
		int id,
		IEmployeesService employeesService,
		CancellationToken cancellationToken)
	{
		var result = await employeesService.GetByCompanyAsync(id, cancellationToken);

		if (result.IsFailure)
		{
			return Results.BadRequest(result);
		}

		return Results.Ok(result.Value);
	}

	private static async Task<IResult> GetByDepartment(
		int id,
		IEmployeesService employeesService,
		CancellationToken cancellationToken)
	{
		var result = await employeesService.GetByDepartmentAsync(id, cancellationToken);

		if (result.IsFailure)
		{
			return Results.BadRequest(result);
		}

		return Results.Ok(result.Value);
	}

	private static async Task<IResult> Delete(
		int id,
		IEmployeesService employeesService,
		CancellationToken cancellationToken)
	{
		var result = await employeesService.DeleteAsync(id, cancellationToken);

		if (result.IsFailure)
		{
			return Results.BadRequest(result);
		}

		return Results.NoContent();
	}

	private static async Task<IResult> Create(
		CreateEmployeeRequest request,
		IEmployeesService employeesService,
		CancellationToken cancellationToken)
	{
		var result = await employeesService.CreateAsync(request, cancellationToken);

		if (result.IsFailure)
		{
			return Results.BadRequest(result);
		}

		return Results.Ok(result.Value);
	}

	private static async Task<IResult> Update(
		[FromRoute] int id,
		[FromBody] UpdateEmployeeRequest request,
		IEmployeesService employeesService,
		CancellationToken cancellationToken)
	{
		request.Id = id;
		var result = await employeesService.UpdateAsync(request, cancellationToken);

		if (result.IsFailure)
			return Results.BadRequest(result);
		
		if (result.Value == 0)
			return Results.NotFound();

		return Results.NoContent();
	}
}

