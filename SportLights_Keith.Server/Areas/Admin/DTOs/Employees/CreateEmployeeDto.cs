using SPORTLIGHTS_SERVER.Constants;
using System.ComponentModel.DataAnnotations;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Employees
{
	public class CreateEmployeeDto
	{
		[Display(Name = "FullName")]
		[Required(ErrorMessage = "{0} is required")]
		[StringLength(ValidatesConstant.MAX_NAME, MinimumLength = ValidatesConstant.MIN_NAME,
		ErrorMessage = "{0} must be at least {2} characters and at most {1} characters")]
		public string FullName { get; set; } = "";

		public DateTime BirthDay { get; set; }

		public string Address { get; set; } = "";

		[StringLength(ValidatesConstant.PHONE_LENGTH,
		ErrorMessage = "{0} must be at most {1} characters")]
		public string Phone { get; set; } = "";

		public string Email { get; set; } = "";

		public string Photo { get; set; } = "";

		public IFormFile? UploadPhoto { get; set; }
	}
}
