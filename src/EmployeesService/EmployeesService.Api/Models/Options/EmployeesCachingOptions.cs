namespace EmployeesService.Api.Models.Options;

public class EmployeesCachingOptions
{
	public const string Section = nameof(EmployeesCachingOptions);

	public required string ByDepartmentKeyPattern { get; set; }
	public required TimeSpan ByDepartmentExpiration { get; set; }
	public required TimeSpan ByDepartmentSlidingExpiration { get; set; }

	public required string ByCompanyKeyPattern { get; set; }
	public required TimeSpan ByCompanyExpiration { get; set; }
	public required TimeSpan ByCompanySlidingExpiration { get; set; }
}
