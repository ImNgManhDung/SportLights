namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Employees
{
	public class EditEmployeeRequestDto
	{
		public int EmployeeId { get; set; }
		public string FullName { get; set; } = default!;
		public string Address { get; set; } = default!;
		public string Email { get; set; } = default!;
		public string? Birthday { get; set; }
		public IFormFile? UploadPhoto { get; set; }
	}

}
