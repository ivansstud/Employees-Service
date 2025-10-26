using FluentValidation;
using FluentValidation.Results;

namespace EmployeesService.Api.Endpoints.EndpointsFilters;

public class ValidationEndpointsFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
{
	private readonly IValidator<TRequest> _validator = validator;

	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		TRequest request = context.Arguments.OfType<TRequest>().First();

		ValidationResult validationResult = await _validator.ValidateAsync(request, context.HttpContext.RequestAborted);

		if (!validationResult.IsValid)
		{
			return TypedResults.ValidationProblem(validationResult.ToDictionary());
		}

		return await next(context);
	}
}
