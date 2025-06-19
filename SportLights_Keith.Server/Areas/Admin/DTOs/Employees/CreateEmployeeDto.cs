namespace SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Employees
{
	public class CreateEmployeeDto
	{

		public string FullName { get; set; } = "";
		public DateTime BirthDate { get; set; }
		public string Address { get; set; } = "";
		public string Phone { get; set; } = "";
		public string Email { get; set; } = "";
		public string Photo { get; set; } = "";
		public bool IsWorking { get; set; }
	}
}
