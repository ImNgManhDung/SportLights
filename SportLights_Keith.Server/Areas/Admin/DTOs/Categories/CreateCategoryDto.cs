using SPORTLIGHTS_SERVER.Attributes.Validations;
using SPORTLIGHTS_SERVER.Constants;
using System.ComponentModel.DataAnnotations;

namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Categories
{
	public class CreateCategoryDto
	{
	
		[Display(Name = "CategoryName")]
		[Required(ErrorMessage = "Yêu cầu nhập dữ liệu {0}")]
		[StringLength(ValidatesConstant.MAX_NAME, MinimumLength = ValidatesConstant.MIN_NAME,
			ErrorMessage = "{0} tối thiểu {2} kí tự và tối đa {1} kí tự")]		
		public string? CategoryName { get; set; }			
		public string? CategoryDescription { get; set; }
	}
}
