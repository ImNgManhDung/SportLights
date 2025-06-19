using SPORTLIGHTS_SERVER.Attributes.Validations;
using System.ComponentModel.DataAnnotations;

namespace SPORTLIGHTS_SERVER.Authen.DTOs
{
	public class LoginDto
	{
		[Required(ErrorMessage = "Tên đăng nhập không được để trống")]
		[MinLength(5, ErrorMessage = "Tên đăng nhập phải có ít nhất 5 ký tự")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Mật khẩu là bắt buộc")]
		[MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
		[MustBeStringAndDigitValidation(ErrorMessage = "Mật khẩu phải có chữ hoa, chữ thường và chữ số")]
		public string Password { get; set; }
	}
}
