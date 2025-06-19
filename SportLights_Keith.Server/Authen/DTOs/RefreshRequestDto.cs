namespace SPORTLIGHTS_SERVER.Authen.DTOs
{
	public class RefreshRequestDto
	{
		public string UserId { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
	}
}
