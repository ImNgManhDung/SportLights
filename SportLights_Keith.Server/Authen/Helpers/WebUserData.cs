using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace SPORTLIGHTS_SERVER.Authen.Helpers
{
	/// <summary>
	/// Thông tin người dùng được lưu trong JWT Token
	/// </summary>
	public class WebUserData
	{
		public string? UserId { get; set; }
		public string? UserName { get; set; }
		public string? DisplayName { get; set; }
		public string? Email { get; set; }
		public string? Photo { get; set; }
		public string? ClientIP { get; set; }
		public string? SessionId { get; set; }
		public string? AdditionalData { get; set; }
		public List<string> Roles { get; set; } = new();

		/// <summary>
		/// Tạo danh sách Claims từ dữ liệu người dùng
		/// </summary>
		public IEnumerable<Claim> Claims
		{
			get
			{
				var claims = new List<Claim>
				{
					new Claim(nameof(UserId), UserId ?? ""),
					new Claim(nameof(UserName), UserName ?? ""),
					new Claim(nameof(DisplayName), DisplayName ?? ""),
					new Claim(nameof(Email), Email ?? ""),
					new Claim(nameof(Photo), Photo ?? ""),
					new Claim(nameof(ClientIP), ClientIP ?? ""),
					new Claim(nameof(SessionId), SessionId ?? ""),
					new Claim(nameof(AdditionalData), AdditionalData ?? "")
				};

				foreach (var role in Roles)
					claims.Add(new Claim(ClaimTypes.Role, role));

				return claims;
			}
		}

		/// <summary>
		/// Tạo ClaimsPrincipal từ dữ liệu người dùng
		/// </summary>
		public ClaimsPrincipal CreatePrincipal()
		{
			var identity = new ClaimsIdentity(Claims, JwtBearerDefaults.AuthenticationScheme);
			return new ClaimsPrincipal(identity);
		}
	}

	public static class WebUserExtensions
	{
		/// <summary>
		/// Lấy thông tin người dùng từ JWT ClaimsPrincipal
		/// </summary>
		public static WebUserData? GetUserData(this ClaimsPrincipal principal)
		{
			if (principal == null || !(principal.Identity?.IsAuthenticated ?? false))
				return null;


			var user = new WebUserData
			{
				UserId = principal.FindFirstValue(nameof(WebUserData.UserId)),
				UserName = principal.FindFirstValue(nameof(WebUserData.UserName)),
				DisplayName = principal.FindFirstValue(nameof(WebUserData.DisplayName)),
				Email = principal.FindFirstValue(nameof(WebUserData.Email)),
				Photo = principal.FindFirstValue(nameof(WebUserData.Photo)),
				ClientIP = principal.FindFirstValue(nameof(WebUserData.ClientIP)),
				SessionId = principal.FindFirstValue(nameof(WebUserData.SessionId)),
				AdditionalData = principal.FindFirstValue(nameof(WebUserData.AdditionalData)),
				Roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
			};

			return user;
		}
	}

	public class WebUserRole
	{
		public WebUserRole(string name, string description)
		{
			Name = name;
			Description = description;
		}

		public string Name { get; set; }
		public string Description { get; set; }
	}

	/// <summary>
	/// Các role mặc định trong hệ thống
	/// </summary>
	public static class WebUserRoles
	{
		public const string Administrator = "admin";
		public const string Moderator = "mod";
		public const string Member = "member";

		public static List<WebUserRole> ListOfRoles => new()
		{
			new WebUserRole(Administrator, "Quản trị hệ thống"),
			new WebUserRole(Moderator, "Cộng tác viên"),
			new WebUserRole(Member, "Thành viên")
		};
	}
}