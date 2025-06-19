namespace SPORTLIGHTS_SERVER.Entities
{
	/// <summary>
	/// thông tin tài khoản 
	/// </summary>
	public class UserAccount
	{
		public string UserId { get; set; } = "";
		public string UserName { get; set; } = "";
		public string FullName { get; set; } = "";
		public string Email { get; set; } = "";
		public string Photo { get; set; } = "";
	}
}
