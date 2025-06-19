//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using SPORTLIGHTS_SERVER.Authen.Helpers;
//using Microsoft.AspNetCore.Authorization;
//using System.Collections.Concurrent;
//using SPORTLIGHTS_SERVER.Constants;
//using System.Reflection;
//using SPORTLIGHTS_SERVER.Authen.DTOs;
//using SPORTLIGHTS_SERVER.Areas.Admin.Repository.CategoryRepository.Abstractions;
//using SPORTLIGHTS_SERVER.Areas.Admin.Repository.CategoryRepository;
//using SPORTLIGHTS_SERVER.Authen.Repository.Auth.Abstractions;
//using SPORTLIGHTS_SERVER.Authen.Repository.Auth;
//using SPORTLIGHTS_SERVER.Entities;

//namespace SPORTLIGHTS_SERVER.Controllers
//{
//	[ApiController]
//	[Route("api/v1/auth")]
//	public class AuthController : ControllerBase
//	{
//		private readonly IConfiguration _config;
//		private static ConcurrentDictionary<string, string> RefreshTokens = new();
//		private static List<string> TokenBlacklist = new();

//		private readonly IAuthRepository _AuthRepo = new AuthRepository();

//		public AuthController(IConfiguration config)
//		{
//			_config = config;
//		}

//		[HttpPost("login")]
//		public IActionResult Login([FromBody] LoginDto dto)
//		{


//			var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
//			var userAccount = _AuthRepo.Authorize(dto.Username, dto.Password, TypeOfAccount.Employee);

//			if (userAccount != null)
//			{
//				// Tạo thông tin user để gắn vào JWT
//				var user = new WebUserData
//				{
//					UserId = userAccount.UserId,
//					UserName = userAccount.UserName,
//					DisplayName = userAccount.FullName,
//					Email = userAccount.Email,
//					Photo = userAccount.Photo,
//					ClientIP = clientIp,
//					SessionId = Guid.NewGuid().ToString(), // vì không dùng HttpContext.Session trong API
//					AdditionalData = "",
//					Roles = new List<string> { WebUserRoles.Administrator } // Hoặc lấy từ DB nếu có bảng Role
//				};

//				var accessToken = GenerateJwtToken(user);
//				var refreshToken = Guid.NewGuid().ToString();
//				RefreshTokens[user.UserId] = refreshToken;

//				return Ok(new
//				{
//					accessToken,
//					refreshToken,
//					userInfo = new
//					{
//						user.UserId,
//						user.UserName,
//						user.DisplayName,
//						user.Email,
//						user.Photo,
//						user.Roles
//					}
//				});
//			}

//			return Unauthorized("Tài khoản hoặc mật khẩu không đúng");
//		}


//		//		if ((dto.Username == "admin" && dto.Password == "123456") || (dto.Username == "user" && dto.Password == "123456"))
//		//	{
//		//		var user = new WebUserData
//		//		{
//		//			UserId = dto.Username == "admin" ? "1" : "2",
//		//			UserName = dto.Username,
//		//			DisplayName = dto.Username == "admin" ? "Administrator" : "User",
//		//			Email = dto.Username == "admin" ? "admin@sportlights.vn" : "user@sportlights.vn",
//		//			Roles = new List<string> { dto.Username == "admin" ? WebUserRoles.Administrator : WebUserRoles.Member }
//		//		};


//		//		var s =  HttpContext.Connection.RemoteIpAddress?.ToString();




//		//		var accessToken = GenerateJwtToken(user);
//		//		var refreshToken = Guid.NewGuid().ToString();
//		//		RefreshTokens[user.UserId] = refreshToken;

//		//		return Ok(new { accessToken, refreshToken });
//		//	}

//		//	return Unauthorized("Tài khoản hoặc mật khẩu không đúng");
//		//}

//			[HttpPost("refresh")]
//			public IActionResult Refresh([FromBody] RefreshRequestDto dto)
//			{
//				if (RefreshTokens.ContainsKey(dto.UserId) && RefreshTokens[dto.UserId] == dto.RefreshToken)
//				{
//					var user = new WebUserData
//					{
//						UserId = dto.UserId,
//						UserName = dto.UserId == "1" ? "admin" : "user",
//						DisplayName = dto.UserId == "1" ? "Administrator" : "User",
//						Email = dto.UserId == "1" ? "admin@sportlights.vn" : "user@sportlights.vn",
//						Roles = new List<string> { dto.UserId == "1" ? WebUserRoles.Administrator : WebUserRoles.Member }
//					};

//					var newAccessToken = GenerateJwtToken(user);
//					return Ok(new { accessToken = newAccessToken });
//				}

//				return Unauthorized("Refresh token không hợp lệ");
//			}

//		[Authorize]
//		[HttpPost("logout")]
//		public IActionResult Logout()
//		{
//			var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
//			if (!string.IsNullOrWhiteSpace(token))
//			{
//				TokenBlacklist.Add(token);
//				return Ok(new
//				{
//					response_code = ResponseCodes.NoContent,
//					results = "Logout",
//				});
//			}
//			return BadRequest("Not Token to Logout");
//		}

//		private string GenerateJwtToken(WebUserData user)
//		{
//			var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
//			var claims = user.CreatePrincipal().Claims;

//			var tokenDescriptor = new SecurityTokenDescriptor
//			{
//				Subject = new ClaimsIdentity(claims),
//				Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"]!)),
//				Issuer = _config["Jwt:Issuer"],
//				Audience = _config["Jwt:Audience"],
//				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//			};

//			var tokenHandler = new JwtSecurityTokenHandler();
//			var token = tokenHandler.CreateToken(tokenDescriptor);
//			return tokenHandler.WriteToken(token);
//		}
//	}



//	public class RefreshRequestDto
//	{
//		public string UserId { get; set; } = string.Empty;
//		public string RefreshToken { get; set; } = string.Empty;
//	}
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SPORTLIGHTS_SERVER.Authen.DTOs;
using SPORTLIGHTS_SERVER.Authen.Helpers;
using SPORTLIGHTS_SERVER.Authen.Repository.Auth;
using SPORTLIGHTS_SERVER.Authen.Repository.Auth.Abstractions;
using SPORTLIGHTS_SERVER.Constants;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
	private readonly IConfiguration _config;

	private static ConcurrentDictionary<string, string> RefreshTokens = new();

	private readonly IAuthRepository _authRepo = new AuthRepository();

	private readonly List<string> _tokenBlacklist;


	public AuthController(IConfiguration config, List<string> tokenBlacklist)
	{
		_config = config;
		_tokenBlacklist = tokenBlacklist;
	}

	[HttpPost("employee-login")]
	public async Task<IActionResult> EmployeeLogin([FromBody] LoginDto dto)
	{
		return await LoginAsync(dto, TypeOfAccount.Employee);
	}

	[HttpPost("customer-login")]
	public async Task<IActionResult> CustomerLogin([FromBody] LoginDto dto)
	{
		return await LoginAsync(dto, TypeOfAccount.Customer);
	}

	private async Task<IActionResult> LoginAsync(LoginDto dto, TypeOfAccount type)
	{
		var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
		var userAccount = await _authRepo.AuthorizeAsync(dto.Username, dto.Password, type);

		if (userAccount != null)
		{
			var user = new WebUserData
			{
				UserId = userAccount.UserId,
				UserName = userAccount.UserName,
				DisplayName = userAccount.FullName,
				Email = userAccount.Email,
				Photo = userAccount.Photo,
				ClientIP = clientIp,
				SessionId = Guid.NewGuid().ToString(),
				AdditionalData = "",
				Roles = new List<string> { type == TypeOfAccount.Employee ? WebUserRoles.Administrator : WebUserRoles.Member }
			};

			var accessToken = GenerateJwtToken(user);
			var refreshToken = Guid.NewGuid().ToString();
			RefreshTokens[user.UserId] = refreshToken;

			return Ok(new
			{
				accessToken,
				refreshToken,
				userInfo = new
				{
					user.UserId,
					user.UserName,
					user.DisplayName,
					user.Email,
					user.Photo,
					user.Roles
				}
			});
		}

		return Unauthorized("Tài khoản hoặc mật khẩu không đúng");
	}

	[Authorize]
	[HttpPost("logout")]
	public Task<IActionResult> Logout()
	{
		var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
		if (!string.IsNullOrEmpty(token))
		{
			_tokenBlacklist.Add(token);
			return Task.FromResult<IActionResult>(Ok(new
			{
				response_code = ResponseCodes.NoContent,
				results = "Đăng xuất thành công"
			}));
		}
		return Task.FromResult<IActionResult>(BadRequest("Không tìm thấy token"));
	}

	[HttpPost("refresh")]
	public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
	{
		if (RefreshTokens.TryGetValue(dto.UserId, out var savedToken) && savedToken == dto.RefreshToken)
		{
			var userAccount = await _authRepo.GetUserByIdAsync(dto.UserId);
			if (userAccount == null)
				return Unauthorized("Không tìm thấy người dùng");

			var user = new WebUserData
			{
				UserId = userAccount.UserId,
				UserName = userAccount.UserName,
				DisplayName = userAccount.FullName,
				Email = userAccount.Email,
				Photo = userAccount.Photo,
				ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
				SessionId = Guid.NewGuid().ToString(),
				AdditionalData = "",
				Roles = new List<string> { WebUserRoles.Administrator }
			};

			var newAccessToken = GenerateJwtToken(user);
			return Ok(new { accessToken = newAccessToken });
		}

		return Unauthorized("Refresh token không hợp lệ");
	}

	private string GenerateJwtToken(WebUserData user)
	{
		var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
		var claims = user.CreatePrincipal().Claims;

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"]!)),
			Issuer = _config["Jwt:Issuer"],
			Audience = _config["Jwt:Audience"],
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};

		var tokenHandler = new JwtSecurityTokenHandler();
		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}
}
