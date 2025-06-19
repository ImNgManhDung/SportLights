using SPORTLIGHTS_SERVER.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SPORTLIGHTS_SERVER.Attributes.Validations
{
	public class MustBeStringAndDigitValidationAttribute : ValidationAttribute
	{
		//public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
		//{
		//    var rule = new ModelClientValidationRule
		//    {
		//        ErrorMessage = FormatErrorMessage(metadata.DisplayName),
		//        ValidationType = "mustbestringanddigit",
		//    };

		//    yield return rule;
		//    }

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null)
				return new ValidationResult("Giá trị không được để trống.");

			var data = value.ToString();

			var regexUpper = new Regex(ValidatesConstant.UPPER);
			var regexLower = new Regex(ValidatesConstant.LOWER);
			var regexDigit = new Regex(ValidatesConstant.DIGIT);

			// The valid condition is: the input must contain at least one uppercase letter, lowercase letter, or digit.
			if (regexUpper.IsMatch(data) || regexLower.IsMatch(data) || regexDigit.IsMatch(data))
			{
				return ValidationResult.Success;
			}

			return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
		}
	}
}