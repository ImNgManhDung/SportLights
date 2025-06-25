	using SPORTLIGHTS_SERVER.Attributes.Validations;
	using SPORTLIGHTS_SERVER.Constants;
	using System.ComponentModel.DataAnnotations;

	namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Categories
	{
		public class CreateCategoryDto
		{

		[Display(Name = "Category Name")]
		[Required(ErrorMessage = "{0} is required")]
		[StringLength(ValidatesConstant.MAX_NAME, MinimumLength = ValidatesConstant.MIN_NAME,
		ErrorMessage = "{0} must be at least {2} characters and at most {1} characters")]
		public string CategoryName { get; set; } = string.Empty;		
		
		public string? CategoryDescription { get; set; }
		}
	}
