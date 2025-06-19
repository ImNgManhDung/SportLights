using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SPORTLIGHTS_SERVER.Middlewares
{
	public class IdempotencyMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IDatabase _redis;

		public IdempotencyMiddleware(RequestDelegate next, IConnectionMultiplexer redis)
		{
			_next = next;
			_redis = redis.GetDatabase();
		}

		public async Task InvokeAsync(HttpContext context)
		{
			// Giới hạn method áp dụng
			if (!HttpMethods.IsPost(context.Request.Method) &&
				!HttpMethods.IsPut(context.Request.Method) &&
				!HttpMethods.IsPatch(context.Request.Method))
			{
				await _next(context);
				return;
			}

			if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey))
			{
				await _next(context); // không có key thì cho qua
				return;
			}

			string userId = context.User?.Identity?.Name ?? "anonymous";
			string cacheKey = $"idempotency:{idempotencyKey}:{userId}";

			// Đọc request body để tính hash
			context.Request.EnableBuffering();
			using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
			var requestBody = await reader.ReadToEndAsync();
			context.Request.Body.Position = 0;

			string requestHash = ComputeSha256Hash(requestBody);

			// Kiểm tra Redis
			var cachedRaw = await _redis.StringGetAsync(cacheKey);
			if (cachedRaw.HasValue)
			{
				var cached = JsonSerializer.Deserialize<IdempotentCache>(cachedRaw!);

				if (cached!.RequestHash != requestHash)
				{
					context.Response.StatusCode = StatusCodes.Status409Conflict;
					context.Response.ContentType = "application/json";
					await context.Response.WriteAsync(JsonSerializer.Serialize(new
					{
						message = "Request already used with a different payload."
					}));
					return;
				}

				context.Response.StatusCode = cached.StatusCode;
				context.Response.ContentType = "application/json";
				await context.Response.WriteAsync(cached.Body);
				return;
			}

			// Capture response
			var originalBodyStream = context.Response.Body;
			using var memStream = new MemoryStream();
			context.Response.Body = memStream;

			await _next(context);

			memStream.Seek(0, SeekOrigin.Begin);
			var bodyText = await new StreamReader(memStream).ReadToEndAsync();

			var responseToCache = new IdempotentCache
			{
				StatusCode = context.Response.StatusCode,
				Body = bodyText,
				RequestHash = requestHash,
				User = userId,
				Timestamp = DateTime.UtcNow
			};

			await _redis.StringSetAsync(
				cacheKey,
				JsonSerializer.Serialize(responseToCache),
				TimeSpan.FromMinutes(10)); // TTL 10 phút

			memStream.Seek(0, SeekOrigin.Begin);
			await memStream.CopyToAsync(originalBodyStream);
		}

		private string ComputeSha256Hash(string input)
		{
			using var sha256 = SHA256.Create();
			var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
			return Convert.ToBase64String(bytes);
		}

		private class IdempotentCache
		{
			public int StatusCode { get; set; }
			public string Body { get; set; } = "";
			public string RequestHash { get; set; } = "";
			public string User { get; set; } = "";
			public DateTime Timestamp { get; set; }
		}
	}
}