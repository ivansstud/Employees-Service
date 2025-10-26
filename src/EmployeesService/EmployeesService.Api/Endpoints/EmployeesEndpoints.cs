using EmployeesService.Api.Extensions;
using EmployeesService.Api.Models.Requests;
using EmployeesService.Api.Services;

namespace EmployeesService.Api.Endpoints;

public class EmployeesEndpoints
{
	public static void MapEndpoints(WebApplication app)
	{
		var authGroup = app.MapGroup("employees");

		authGroup.MapPost("/", CreateHandler)
			.WithValidationFilter<CreateEmployeeRequest>();

		authGroup.MapDelete("/{id}", DeleteHandler);
	}

	private static async Task<IResult> DeleteHandler(
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

	private static async Task<IResult> CreateHandler(
		CreateEmployeeRequest request,
		IEmployeesService employeesService,
		CancellationToken cancellationToken)
	{
		var result = await employeesService.Create(request, cancellationToken);

		if (result.IsFailure)
		{
			return Results.BadRequest(result);
		}

		return Results.Ok(result.Value);
	}
}
