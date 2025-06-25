using System.ComponentModel.DataAnnotations;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Suppliers
{
	public class CreateSupplierDto
	{

		[Required(ErrorMessage = "Yêu cầu nhập dữ liệu {0}")]
		public string SupplierName { get; set; } = string.Empty;

		public string ContactName { get; set; } = string.Empty;

		public string Provice { get; set; } = string.Empty;

		public string Address { get; set; } = string.Empty;

		public string Phone { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;
	}
}
