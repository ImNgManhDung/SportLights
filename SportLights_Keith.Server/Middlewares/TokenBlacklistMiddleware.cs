namespace SPORTLIGHTS_SERVER.Middlewares
{
	public class TokenBlacklistMiddleware
	{
		private readonly RequestDelegate _next;
		private static List<string> _blacklist;

		public TokenBlacklistMiddleware(RequestDelegate next, List<string> tokenBlacklist)
		{
			_next = next;
			_blacklist = tokenBlacklist;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

			if (!string.IsNullOrEmpty(token) && _blacklist.Contains(token))
			{
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				await context.Response.WriteAsync("Token đã bị thu hồi (logged out)");
				return;
			}

			await _next(context);
		}
	}

}
