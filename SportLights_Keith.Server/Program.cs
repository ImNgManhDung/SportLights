using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SPORTLIGHTS_SERVER.Middlewares;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var redisHost = builder.Configuration["Redis:Host"];
var redisPort = builder.Configuration["Redis:Port"];
var redisPassword = builder.Configuration["Redis:Password"];

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
	};

	options.Events = new JwtBearerEvents
	{
		OnChallenge = context =>
		{
			context.HandleResponse();
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			context.Response.ContentType = "application/json";
			var result = new
			{
				response_code = 403,
				results = "Forbidden"
			};
			return context.Response.WriteAsync(JsonSerializer.Serialize(result));
		},
		OnForbidden = context =>
		{
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			context.Response.ContentType = "application/json";
			var result = new
			{
				response_code = 403,
				results = "Forbidden"
			};
			return context.Response.WriteAsync(JsonSerializer.Serialize(result));
		}

	};
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var tokenBlacklist = new List<string>();
builder.Services.AddSingleton(tokenBlacklist);


// Builder Redis


builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
	var muxer = ConnectionMultiplexer.Connect(
						new ConfigurationOptions
						{
							EndPoints = { { "redis-12116.crce178.ap-east-1-1.ec2.redns.redis-cloud.com", 12116 } },
							User = "default",
							Password = "bdSVlLpBZmRfStFrzZKXn7IgqcURjusq"
						}
					);
	return muxer;
});

builder.Services.AddSingleton<RedisCacheService>();


// CORS policy
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

/// app redis
app.UseMiddleware<IdempotencyMiddleware>();
app.UseMiddleware<RateLimiterMiddleware>();


app.UseMiddleware<TokenBlacklistMiddleware>();

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
