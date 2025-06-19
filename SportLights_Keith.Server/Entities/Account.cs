namespace SPORTLIGHTS_SERVER.Entities
{
	public class Account
	{

		public int IdAccount { get; set; }
		public string? AccountName { get; set; } = "";

		public string? AccountUser { get; set; } = "";

		public string? Pass { get; set; } = "";
		public string? GetPass()
		{
			return Pass;
		}
		public string? GetAccountUser()
		{
			return AccountUser;
		}
		public int? GetIdAccount()
		{
			return IdAccount;
		}

	}
}
