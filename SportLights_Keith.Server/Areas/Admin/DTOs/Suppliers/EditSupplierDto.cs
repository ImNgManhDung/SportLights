namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Suppliers
{
	public class EditSupplierDto
	{
		public int SupplierId { get; set; }
		public string SupplierName { get; set; } = string.Empty;
		public string ContactName { get; set; } = string.Empty;
		public string Provice { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
	}
}
