using SPORTLIGHTS_SERVER.Constants;
using System.ComponentModel.DataAnnotations;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Shippers
{
	public class CreateShipperDto
	{
		[Display(Name = "ShipperName")]
		[Required(ErrorMessage = "{0} is required")]
		[StringLength(ValidatesConstant.MAX_NAME, MinimumLength = ValidatesConstant.MIN_NAME,
	ErrorMessage = "{0} must be at least {2} characters and at most {1} characters")]
		public string ShipperName { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;

	}
}
