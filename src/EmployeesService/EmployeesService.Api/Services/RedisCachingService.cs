using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EmployeesService.Api.Services;

public interface IRedisCachingService
{
	Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);
	Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken);
	Task RemoveAsync(string key, CancellationToken cancellationToken);
}

public class RedisCachingService(IDistributedCache distributedCache) : IRedisCachingService
{
	private readonly IDistributedCache _distributedCache = distributedCache;

	public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
	{
		string? cachedData = await _distributedCache.GetStringAsync(key, cancellationToken);

		if (cachedData is not null)
		{
			return JsonSerializer.Deserialize<T>(cachedData);
		}

		return default;
	}

	public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken)
	{
		await _distributedCache.SetStringAsync(
			key,
			JsonSerializer.Serialize(value),
			options,
			cancellationToken);
	}

	public async Task RemoveAsync (string key, CancellationToken cancellationToken)
	{
		await _distributedCache.RemoveAsync(key, cancellationToken);
	}
}