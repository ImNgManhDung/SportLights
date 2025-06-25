using SPORTLIGHTS_SERVER.Constants;
using System.ComponentModel.DataAnnotations;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Customers
{
	public class CreateCustomerDto
	{
		[Display(Name = "CustomerName")]
		[Required(ErrorMessage = "{0} is required")]
		[StringLength(ValidatesConstant.MAX_NAME, MinimumLength = ValidatesConstant.MIN_NAME,
		ErrorMessage = "{0} must be at least {2} characters and at most {1} characters")]
		public string CustomerName { get; set; } = string.Empty;

		public string ContactName { get; set; } = string.Empty;

		public string Province { get; set; } = string.Empty;

		public string Address { get; set; } = string.Empty;

		public string Phone { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public bool IsLocked { get; set; }
	}
}
