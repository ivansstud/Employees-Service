using EmployeesService.Api.Endpoints.EndpointsFilters;

namespace EmployeesService.Api.Extensions;

public static class RouteHandlerBuilderExtensions
{
	public static RouteHandlerBuilder WithValidationFilter<TRequest>(this RouteHandlerBuilder builder)
	{
		builder.AddEndpointFilter<ValidationEndpointsFilter<TRequest>>()
			.ProducesValidationProblem();

		return builder;
	}
}
