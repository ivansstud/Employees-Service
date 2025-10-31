using EmployeesService.Api.Data.Entities;
using EmployeesService.Api.Models.Options;
using EmployeesService.Api.Models.Responses;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace EmployeesService.Api.Services;

public interface IEmployeesCacheService
{
	Task<IEnumerable<EmployeeResponse>?> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken);
	Task SetByDepartmentAsync(int departmentId, IEnumerable<EmployeeResponse> employees, CancellationToken cancellationToken);
	Task<IEnumerable<EmployeeResponse>?> GetByCompanyAsync(int companyId, CancellationToken cancellationToken);
	Task SetByCompanyAsync(int companyId, IEnumerable<EmployeeResponse> employees, CancellationToken cancellationToken);
}

public class EmployeesCacheService(
	IRedisCachingService cache,
	IOptions<EmployeesCachingOptions> options) : IEmployeesCacheService
{
	private readonly IRedisCachingService _cache = cache;
	private readonly EmployeesCachingOptions _options = options.Value;

	public async Task<IEnumerable<EmployeeResponse>?> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken)
	{
		var cacheKey = GetDepartmentCacheKey(departmentId);
		return await _cache.GetAsync<IEnumerable<EmployeeResponse>>(cacheKey, cancellationToken);
	}

	public async Task SetByDepartmentAsync(int departmentId, IEnumerable<EmployeeResponse> employees, CancellationToken cancellationToken)
	{
		string cacheKey = GetDepartmentCacheKey(departmentId);

		var cacheOptions = new DistributedCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = _options.ByDepartmentExpiration,
			SlidingExpiration = _options.ByDepartmentSlidingExpiration
		};

		await _cache.SetAsync(cacheKey, employees, cacheOptions, cancellationToken);
	}

	public async Task<IEnumerable<EmployeeResponse>?> GetByCompanyAsync(int companyId, CancellationToken cancellationToken)
	{
		var cacheKey = GetCompanyCacheKey(companyId);
		return await _cache.GetAsync<IEnumerable<EmployeeResponse>>(cacheKey, cancellationToken);
	}

	public async Task SetByCompanyAsync(int companyId, IEnumerable<EmployeeResponse> employees, CancellationToken cancellationToken)
	{
		string cacheKey = GetCompanyCacheKey(companyId);

		var cacheOptions = new DistributedCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = _options.ByCompanyExpiration,
			SlidingExpiration = _options.ByCompanySlidingExpiration
		};

		await _cache.SetAsync(cacheKey, employees, cacheOptions, cancellationToken);
	}

	public async Task RemoveByCompanyAsync(int companyId, CancellationToken cancellationToken)
	{
		var cacheKey = GetCompanyCacheKey(companyId);
		await _cache.RemoveAsync(cacheKey, cancellationToken);
	}

	private string GetCompanyCacheKey(int companyId)
	{
		return string.Format(_options.ByCompanyKeyPattern, companyId);
	}

	private string GetDepartmentCacheKey(int departmentId)
	{
		return string.Format(_options.ByDepartmentKeyPattern, departmentId);
	}
}