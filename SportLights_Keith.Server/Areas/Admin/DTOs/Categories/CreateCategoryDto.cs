using SPORTLIGHTS_SERVER.Attributes.Validations;
using SPORTLIGHTS_SERVER.Constants;
using System.ComponentModel.DataAnnotations;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Categories
{
	public class CreateCategoryDto
	{
		public int CategoryId { get; set; }

		[Display(Name = "Loại Hang")]
		[Required(ErrorMessage = "Yêu cầu nhập dữ liệu {0}")]
		[StringLength(ValidatesConstant.MAX_FOOD_NAME, MinimumLength = ValidatesConstant.MIN_FOOD_NAME,
			ErrorMessage = "{0} tối thiểu {2} kí tự và tối đa {1} kí tự")]
		[MustBeStringAndDigitValidation(ErrorMessage = "{0} yêu cầu phải là kiểu ký tự dạng chữ hoặc dạng số")]
		public string? CategoryName { get; set; }
		public string? CategoryDescription { get; set; }
	}
}
