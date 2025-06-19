namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Customers
{
	public class CreateCustomerDto
	{
		public string CustomerName { get; set; } = "";
		public string ContactName { get; set; } = "";
		public string Province { get; set; } = "";
		public string Address { get; set; } = "";
		public string Phone { get; set; } = "";
		public string Email { get; set; } = "";
		public bool IsLocked { get; set; }
	}
}
