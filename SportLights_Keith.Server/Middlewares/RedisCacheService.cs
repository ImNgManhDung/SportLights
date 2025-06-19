using StackExchange.Redis;
using System.Text.Json;

public class RedisCacheService
{
	private readonly IDatabase _db;

	public RedisCacheService(IConnectionMultiplexer redis)
	{
		_db = redis.GetDatabase();
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
}
