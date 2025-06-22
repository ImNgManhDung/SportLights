using StackExchange.Redis;
using System.Text.Json;

public class RedisCacheService
{
	private readonly IDatabase _db;
	private readonly IConnectionMultiplexer _redis;

	public RedisCacheService(IConnectionMultiplexer redis)
	{
		_db = redis.GetDatabase();
		_redis = redis;
	}

	public async Task<T?> GetCacheAsync<T>(string key)
	{
		var value = await _db.StringGetAsync(key);
		return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
	}

	public async Task SetCacheAsync<T>(string key, T value, TimeSpan? expiry = null)
	{
		var data = JsonSerializer.Serialize(value);
		await _db.StringSetAsync(key, data, expiry);
	}

	public async Task RemoveCategoryCacheAsync(string pattern)
	{
		var server = _redis.GetServer(_redis.GetEndPoints().First());
		foreach (var key in server.Keys(pattern: $"{pattern}:*"))
		{
			await _db.KeyDeleteAsync(key);
		}
	}

	public async Task<IEnumerable<RedisKey>> GetKeysByPatternAsync(string pattern)
	{
		return await Task.Run(() =>
		{
			var server = _redis.GetServer(_redis.GetEndPoints().First());
			return server.Keys(pattern: pattern).ToList(); 
		});
	}

	public async Task SetCacheWithIdsAsync<T>(string key, T value, List<int> relatedIds, TimeSpan? expiry = null)
	{
		await SetCacheAsync(key, value, expiry);
		
		string idKey = $"cache_ids:{key}";
		var idData = JsonSerializer.Serialize(relatedIds);
		await _db.StringSetAsync(idKey, idData, expiry);
	}

	public async Task InvalidateCacheByAffectedIdAsync(int id)
	{
		var keys = await GetKeysByPatternAsync("cache_ids:*");
		foreach (var idKey in keys)
		{
			var value = await _db.StringGetAsync(idKey);
			if (!value.HasValue) continue;

			var idList = JsonSerializer.Deserialize<List<int>>(value!);
			if (idList?.Contains(id) == true)
			{				
				var originalKey = idKey.ToString().Replace("cache_ids:", "");
				await _db.KeyDeleteAsync(originalKey);
				await _db.KeyDeleteAsync(idKey);
			}
		}

	}
}
