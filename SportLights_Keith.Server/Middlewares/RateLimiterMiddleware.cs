using StackExchange.Redis;

public class RateLimiterMiddleware
{
	private readonly RequestDelegate _next;
	private readonly IDatabase _redis;

	public RateLimiterMiddleware(RequestDelegate next, IConnectionMultiplexer redis)
	{
		_next = next;
		_redis = redis.GetDatabase();
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var userId = context.User?.Identity?.Name ?? "anonymous";
		var key = $"ratelimit:{userId}:{DateTime.UtcNow:yyyyMMddHHmm}"; // reset mỗi phút

		var count = await _redis.StringIncrementAsync(key);
		if (count == 1)
			await _redis.KeyExpireAsync(key, TimeSpan.FromMinutes(1));

		if (count > 5)
		{
			context.Response.StatusCode = 429;
			await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
			return;
		}

		await _next(context);
	}
}
